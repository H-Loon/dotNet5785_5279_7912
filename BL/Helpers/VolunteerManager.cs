using DalApi;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Retrieves a list of volunteers based on their active status.
    /// </summary>
    /// <param name="active">The active status to filter volunteers by. If null, all volunteers are returned.</param>
    /// <returns>A list of volunteers matching the specified active status.</returns>
    internal static IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? active)
    {
        try
        {
            IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll();
            IEnumerable<DO.Call> calls = s_dal.Call.ReadAll();

            return from v in s_dal.Volunteer.ReadAll() // create a list of BO.Volunteers
                   where active == null || v.IsActive == active
                   let complCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.Completed)
                   let canceledCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.CanceledByVolunteer)
                   let callInTreatmentId = assignments.FirstOrDefault(a => a.VolunteerId == v.Id && a.EndReason == null)?.CallId
                   let callInTreatmentType = calls.FirstOrDefault(c => c.Id == callInTreatmentId)?.Type
                   select new BO.VolunteerInList
                   {
                       Id = v.Id,
                       Name = v.Name,
                       IsActive = v.IsActive,
                       CompletedCalls = complCalls,
                       CanceledCalls = canceledCalls,
                       CallInTreatment = callInTreatmentId,
                       CurrentCallType = callInTreatmentType.HasValue ? (BO.BoCallType)callInTreatmentType : BO.BoCallType.None
                   };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Fills the volunteer's latitude and longitude based on their address and hashes their password.
    /// </summary>
    /// <param name="volunteer">The volunteer object to be filled.</param>
    internal static void DOVolunteerFiller(BO.Volunteer volunteer)
    {
        if (volunteer.Address is not null) // Ai helped
        {
            (volunteer.Latitude, volunteer.Longitude) = Tools.AddressToCoordinates(volunteer.Address);
        }

        if (volunteer.Password != null)
            volunteer.Password = CryptPW(volunteer.Password);
    }

    /// <summary>
    /// Fills the passwords for all volunteers initialized in the database by hashing them.
    /// </summary>
    internal static void PasswordFillerForInit()
    {
        var volunteers = s_dal.Volunteer.ReadAll();

        foreach (var v in volunteers) // Encrypt all passwords of initialized volunteers
        {
            var password = CryptPW(v.Password);
            s_dal.Volunteer.Update(new DO.Volunteer
            {
                Id = v.Id,
                Name = v.Name,
                Phone = v.Phone,
                Email = v.Email,
                Password = password,
                Address = v.Address,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                Role = v.Role,
                IsActive = v.IsActive,
                MaxDistance = v.MaxDistance,
                DistanceType = v.DistanceType
            });
        }
    }

    /// <summary>
    /// Converts a DO.Volunteer object to a BO.Volunteer object by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be converted.</param>
    /// <returns>The converted BO.Volunteer object.</returns>
    internal static BO.Volunteer ConvertToBO(int id)
    {
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll();

        DO.Volunteer v = s_dal.Volunteer.Read(id) ?? throw new BO.BlNotExistException($"No volunteer with ID = {id} found");

        BO.CallInProgress? callInProgress = null;

        int openCallId = assignments.FirstOrDefault(a => a.VolunteerId == v.Id && a.EndReason == null)?.CallId ?? 0;

        if (openCallId is not 0) // If the volunteer has an open call in progress 
        {
            var assignment = assignments.FirstOrDefault(a => a.VolunteerId == v.Id && a.EndReason == null);
            var call = s_dal.Call.Read(c => c.Id == openCallId);
            callInProgress = new BO.CallInProgress
            {
                AssignmentId = assignment!.Id,
                CallId = openCallId,
                CallType = (BO.BoCallType)call!.Type,
                Description = call.Description,
                Address = call.Address,
                StartTime = call.StartTime,
                MaxTime = call.MaxTime,
                AssignTime = assignment.StartTime,
                CallDistance = Tools.GetCallDistance(openCallId, v),
                Status = CallManager.GetCallStatus(openCallId)
            };
        }

        return new BO.Volunteer 
        {
            Id = v.Id,
            Name = v.Name,
            Phone = v.Phone,
            Email = v.Email,
            Password = v.Password,
            Address = v.Address,
            Latitude = v.Latitude,
            Longitude = v.Longitude,
            Role = (BO.BoRoleType)v.Role,
            IsActive = v.IsActive,
            MaxDistance = v.MaxDistance,
            DistanceType = (BO.BoDistanceType)v.DistanceType,
            CompletedCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.Completed),
            CanceledCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.CanceledByVolunteer),
            OverDatedCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.OverDated),
            CurrentCall = callInProgress
        };
    }

    /// <summary>
    /// Converts a BO.Volunteer object to a BO.Volunteer object.
    /// </summary>
    /// <param name="v">The BO.Volunteer object to be converted.</param>
    /// <returns>The converted BO.Volunteer object.</returns>
    internal static BO.Volunteer ConvertToBo(BO.Volunteer v)
    {
        return ConvertToBO(v.Id);
    }

    /// <summary>
    /// Converts a BO.Volunteer object to a DO.Volunteer object.
    /// </summary>
    /// <param name="volunteer">The BO.Volunteer object to be converted.</param>
    /// <returns>The converted DO.Volunteer object.</returns>
    internal static DO.Volunteer ConvertToDO(BO.Volunteer volunteer)
    {
        return new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (DO.RoleType)volunteer.Role,
            IsActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceType = (DO.DistanceType)volunteer.DistanceType
        };
    }

    /// <summary>
    /// Validates the properties of a BO.Volunteer object.
    /// </summary>
    /// <param name="volunteer">The BO.Volunteer object to be validated.</param>
    /// <returns>True if all properties are valid, otherwise false.</returns>
    internal static bool BOVolunteerCheck(BO.Volunteer volunteer)
    {
        if (IdCheck(volunteer.Id) is false)
            return false;
        if (NameCheck(volunteer.Name) is false)
            return false;
        if (PhoneCheck(volunteer.Phone) is false)
            return false;
        if (EmailCheck(volunteer.Email) is false)
            return false;
        if (PasswordCheck(volunteer.Password!) is false)
            return false;
        if (Tools.AddressCheck(volunteer.Address) is false)
            return false;
        if (MaxDistanceCheck(volunteer.MaxDistance) is false)
            return false;
        return true;
    }

    /// <summary>
    /// Checks if the given ID is valid.
    /// </summary>
    /// <param name="id">The ID to be checked.</param>
    /// <returns>True if the ID is valid, otherwise false.</returns>
    internal static bool IdCheck(int id) // AI helped
    {
        {
            string idString = id.ToString();

            if (idString.Length != 9)
                return false;

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(idString[i].ToString()); // Get the digit at the current index
                int product = digit * (i % 2 == 0 ? 1 : 2); // Multiply the digit by 1 if the index is even, otherwise multiply it by 2
                sum += product > 9 ? product - 9 : product; // If the product is greater than 9, subtract 9 from it to get the sum of its digits (e.g. 12 - 9 = 1 + 2)
            }

            return sum % 10 == 0; // The ID is valid if the sum is divisible by 10
        }
    }

    /// <summary>
    /// Checks if the given password is strong enough.
    /// </summary>
    /// <param name="password">The password to be checked.</param>
    /// <returns>True if the password is valid, otherwise false.</returns>
    internal static bool PasswordCheck(string password) // A Good Password is at least 6 characters long, contains at least one uppercase letter, one lowercase letter, one digit, and one special character
    {
        char[] specialCharacters = 
            { '@', '!', '?', '#', '$', '%',
            '^', '&', '*', '(', ')', '-', '_',
            '=', '+', '[', ']','{', '}', '|',
            '\\', ':', ';', '"', '\'', '<', '>',
             ',','.', '/', '~', '`' };

        if (password is null)
            return true;
        if (password.Length < 6)
            return false;
        if (password.IndexOfAny(specialCharacters) is -1)
            return false;
        if (password.Any(char.IsUpper) is false)
            return false;
        if (password.Any(char.IsLower) is false)
            return false;
        if (password.Any(char.IsDigit) is false)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the given email is valid.
    /// </summary>
    /// <param name="email">The email to be checked.</param>
    /// <returns>True if the email is valid, otherwise false.</returns>
    internal static bool EmailCheck(string email) // Ai helped
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Pattern to match email addresses (e.g. "abc@abc.abc")
        string pattern = @"^[^@\s]+@[a-z]+\.[a-z]+$";
        return Regex.IsMatch(email, pattern);
    }

    /// <summary>
    /// Checks if the given phone number is valid.
    /// </summary>
    /// <param name="pn">The phone number to be checked.</param>
    /// <returns>True if the phone number is valid, otherwise false.</returns>
    internal static bool PhoneCheck(string pn)
    {
        string pattern = @"^\+972-[0-9]{2}-[0-9]{7}$"; // (e.g. +972-55-1234567)
        return Regex.IsMatch(pn, pattern);
    }

    /// <summary>
    /// Checks if the given name is valid.
    /// </summary>
    /// <param name="name">The name to be checked.</param>
    /// <returns>True if the name is valid, otherwise false.</returns>
    internal static bool NameCheck(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        string pattern = @"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$"; // Each word starts with an uppercase letter followed by one or more lowercase letters, separated by spaces (e.g. "John Doe")
        return Regex.IsMatch(name, pattern);
    }

    /// <summary>
    /// Checks if the given maximum distance is valid.
    /// </summary>
    /// <param name="maxDistance">The maximum distance to be checked.</param>
    /// <returns>True if the maximum distance is valid, otherwise false.</returns>
    internal static bool MaxDistanceCheck(double? maxDistance)
    {
        if (maxDistance is null)
            return true;
        if (maxDistance < 0 || maxDistance > 500) // The maximum distance must be between 0 and 500 kilometers
            return false;
        return true;
    }

    /// <summary>
    /// Hashes a given password using the SHA256 algorithm and returns the hashed password as a hexadecimal string.
    /// </summary>
    /// <param name="password">The plain text password to be hashed.</param>
    /// <returns>The hashed password as a hexadecimal string, or null if the input password is null.</returns>
    internal static string? CryptPW(string? password) // Ai helped
    {
        // Check if the password is null, return null if it is
        if (password is null)
            return null;

        // Create a new instance of the SHA256 cryptographic service provider
        using (SHA256 sha256 = SHA256.Create())
        {
            // Compute the hash of the password, converting it to a byte array
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Initialize a StringBuilder to collect the bytes and convert them to a hexadecimal string
            StringBuilder builder = new StringBuilder();

            // Iterate over each byte in the byte array
            foreach (byte b in bytes)
            {
                // Convert the byte to a hexadecimal string and append it to the StringBuilder
                builder.Append(b.ToString("x2"));
            }

            // Return the final hashed password as a hexadecimal string
            return builder.ToString();
        }
    }
}
