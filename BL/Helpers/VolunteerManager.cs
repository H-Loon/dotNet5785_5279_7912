using DalApi;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? active)
    {
        try
        {
            IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll();
            IEnumerable<DO.Call> calls = s_dal.Call.ReadAll();
            return from v in s_dal.Volunteer.ReadAll()
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
                       CurrentCallType = callInTreatmentType.HasValue ? (BO.BoCallType)callInTreatmentType : BO.BoCallType.None
                   };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    internal static void DOVolunteerFiller(BO.Volunteer volunteer)
    {
        if (volunteer.Address is not null) // Ai helped
        {
            (volunteer.Latitude, volunteer.Longitude) = Tools.AddressToCoordinates(volunteer.Address);
        }

        if (volunteer.Password != null)
            volunteer.Password = CryptPW(volunteer.Password);
    }
    internal static void PasswordFillerForInit()
    {
        var volunteers = s_dal.Volunteer.ReadAll();
        foreach (var v in volunteers)
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
    internal static BO.Volunteer ConvertToBO(int id)
    {
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll();

        DO.Volunteer v = s_dal.Volunteer.Read(id) ?? throw new Exception($"No volunteer with ID = {id} found");

        BO.CallInProgress? callInProgress = null;

        int callId = assignments.FirstOrDefault(a => a.VolunteerId == v.Id && a.EndReason == null)?.CallId ?? 0;

        if (callId is not 0)
        {
            var assignment = assignments.FirstOrDefault(a => a.VolunteerId == v.Id && a.EndReason == null);
            var call = s_dal.Call.Read(c => c.Id == callId);
            callInProgress = new BO.CallInProgress
            {
                Id = assignment!.Id,
                CallId = callId,
                CallType = (BO.BoCallType)call!.Type,
                Description = call.Description,
                Address = call.Address,
                StartTime = call.StartTime,
                MaxTime = call.MaxTime,
                AssignTime = assignment.StartTime,
                CallDistance = Tools.GetCallDistance(callId, v),
                Status = Tools.GetCallStatus(callId)
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
    internal static BO.Volunteer ConvertToBo(BO.Volunteer v)
    {
        return ConvertToBO(v.Id);
    }
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
    internal static bool IdCheck(int id) // AI helped
    {
        {
            string idString = id.ToString();

            // Ensure the ID has 9 digits
            idString = idString.PadLeft(9, '0');

            if (idString.Length != 9 || !int.TryParse(idString, out _))
                return false;

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(idString[i].ToString());
                int product = digit * (i % 2 == 0 ? 1 : 2);
                sum += product > 9 ? product - 9 : product;
            }

            return sum % 10 == 0;
        }
    }

    internal static bool PasswordCheck(string password)
    {
        char[] specialCharacters = { '@', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', ']', '{', '}', '|', '\\', ':', ';', '"', '\'', '<', '>', ',', '.', '/', '~', '`' };

        if (password is null)
            return true;
        if (password.Length < 6)
            return false;
        if (password.IndexOfAny(specialCharacters) >= 0)
            return false;
        if (password.Any(char.IsUpper) is false)
            return false;
        if (password.Any(char.IsLower) is false)
            return false;
        if (password.Any(char.IsDigit) is false)
            return false;

        return true;
    }

    internal static bool EmailCheck(string email) // Ai helped
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Pattern to match email addresses
        string pattern = @"^[^@\s]+@[a-z]+\.[a-z]+$";
        return Regex.IsMatch(email, pattern);
    }

    internal static bool PhoneCheck(string pn)
    {
        string pattern = @"^\+972-[0-9]{2}-[0-9]{7}$";
        return Regex.IsMatch(pn, pattern);
    }

    internal static bool NameCheck(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        string pattern = @"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$"; // Each word starts with an uppercase letter followed by one or more lowercase letters, separated by spaces
        return Regex.IsMatch(name, pattern);
    }

    internal static bool MaxDistanceCheck(double? maxDistance)
    {
        if (maxDistance is null)
            return true;
        if (maxDistance < 0 || maxDistance > 500)
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
