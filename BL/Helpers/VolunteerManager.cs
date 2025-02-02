using BlImplementation;
using BO;
using DalApi;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    private static List<OpenCallInList> s_list;
    private static TimeSpan s_minTime = new(0, 1, 0, 0);
    private static readonly Random s_rand = new();

    internal static ObserverManager Observers = new(); //stage 5

    /// <summary>
    /// Retrieves a list of s_volunteers based on their active status.
    /// </summary>
    /// <param name="active">The active status to filter s_volunteers by. If null, all s_volunteers are returned.</param>
    /// <returns>A list of s_volunteers matching the specified active status.</returns>
    internal static IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? active)
    {
        try
        {
            IEnumerable<BO.VolunteerInList>? volunteers;
            lock (AdminManager.BlMutex) //stage 7
            { 
                IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll().ToList();
                IEnumerable<DO.Call> calls = s_dal.Call.ReadAll().ToList();

                volunteers = from v in s_dal.Volunteer.ReadAll().ToList() // create a list of BO.Volunteers
                             where active == null || v.IsActive == active
                       let complCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.Completed)
                       let canceledCalls = assignments.Count(a => a.VolunteerId == v.Id && a.EndReason == DO.AssignmentEndReason.CanceledByVolunteer)
                       let callInTreatmentId = assignments.LastOrDefault(a => a.VolunteerId == v.Id && a.EndReason == null)?.CallId
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
                volunteers = volunteers.ToList();
            }
            return volunteers;
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
    internal static async Task DOVolunteerFiller(BO.Volunteer volunteer , bool flag1, bool flag2)
    {
        volunteer.Password = null;// Default value for password

        if (flag2 && volunteer.Address is not null) // Ai helped
        {
            (volunteer.Latitude, volunteer.Longitude) = await Tools.AddressToCoordinatesAsync(volunteer.Address); // Get the coordinates of the address
        }

        if (flag1)
            volunteer.Password = CryptPW(volunteer.Password); // Hash the password
    }

    /// <summary>
    /// Fills the passwords for all s_volunteers initialized in the database by hashing them.
    /// </summary>
    internal static void PasswordFillerForInit()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex) //stage 7 
            volunteers = s_dal.Volunteer.ReadAll().ToList();

        foreach (var v in volunteers) // Encrypt all passwords of initialized s_volunteers
        {
            var password = CryptPW(v.Password);

            lock (AdminManager.BlMutex) //stage 7
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
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Converts a DO.Volunteer object to a BO.Volunteer object by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be converted.</param>
    /// <returns>The converted BO.Volunteer object.</returns>
    internal static BO.Volunteer ConvertToBO(int id)
    {
        DO.Volunteer v;
        lock (AdminManager.BlMutex) //stage 7
            v = s_dal.Volunteer.Read(id) ?? throw new BO.BlNotExistException($"No volunteer with ID = {id} found");
        
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id && a.EndReason == null).ToList();

        BO.CallInProgress? callInProgress = null;

        int openCallId = assignments.FirstOrDefault()?.CallId ?? 0;

        if (openCallId is not 0) // If the volunteer has an open call in progress 
        {
            var assignment = assignments.First();

            DO.Call? call;
            lock (AdminManager.BlMutex) //stage 7
                call = s_dal.Call.Read(c => c.Id == openCallId);

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
            CompletedCalls = assignments.Count(a => a.EndReason == DO.AssignmentEndReason.Completed),
            CanceledCalls = assignments.Count(a => a.EndReason == DO.AssignmentEndReason.CanceledByVolunteer),
            OverDatedCalls = assignments.Count(a => a.EndReason == DO.AssignmentEndReason.OverDated),
            CurrentCall = callInProgress
        };
    }

    /// <summary>
    /// Converts a BO.Volunteer object to a BO.Volunteer object.
    /// </summary>
    /// <param name="v">The BO.Volunteer object to be converted.</param>
    /// <returns>The converted BO.Volunteer object.</returns>
    private static BO.Volunteer ConvertToBo(DO.Volunteer v)
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
    internal static void BOVolunteerCheck(BO.Volunteer volunteer , bool flag1 , bool flag2)
    {
        if (flag1 && IdCheck(volunteer.Id) is false)
            throw new BO.BlNotValidEntityException("Volunteer Id is not valid");
        if (NameCheck(volunteer.Name) is false)
            throw new BO.BlNotValidEntityException("Volunteer Name is not valid");
        if (PhoneCheck(volunteer.Phone) is false)
            throw new BO.BlNotValidEntityException("Volunteer Phone is not valid");
        if (EmailCheck(volunteer.Email) is false)
            throw new BO.BlNotValidEntityException("Volunteer Email is not valid");
        if (PasswordCheck(volunteer.Password!) is false)
            throw new BO.BlNotValidEntityException("Volunteer Password is not valid");
        if (flag2 && Tools.AddressCheck(volunteer.Address) is false)
            throw new BO.BlNotValidEntityException("Volunteer Address is not valid");
        if (MaxDistanceCheck(volunteer.MaxDistance) is false)
            throw new BO.BlNotValidEntityException("Volunteer Max Distance is not valid");
    }

    /// <summary>
    /// Checks if the given ID is valid.
    /// </summary>
    /// <param name="id">The ID to be checked.</param>
    /// <returns>True if the ID is valid, otherwise false.</returns>
    internal static bool IdCheck(int id) // AI helped
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

        if (string.IsNullOrEmpty(password))
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
        // Remove all white space
        email = email.Trim();
        // Pattern to match email addresses (e.g. "abc@abc.abc")
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    /// <summary>
    /// Checks if the given phone number is valid.
    /// </summary>
    /// <param name="pn">The phone number to be checked.</param>
    /// <returns>True if the phone number is valid, otherwise false.</returns>
    internal static bool PhoneCheck(string pn)
    {
        string pattern = @"^0[\d]{9}$"; // (e.g. 0551234567)
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

    internal static void SimulFunction()
    {
        IEnumerable<BO.VolunteerInList>? volunteers;
        IEnumerable<DO.Assignment>? assignments;
        
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll().ToList();

        lock (AdminManager.BlMutex) //stage 7
            volunteers = GetVolunteerInLists(true).ToList();
        try
        {
            foreach (var v in volunteers)
            {
                if (v.CallInTreatment is not null)
                {
                    var assignment = assignments.Last(a => a.CallId == v.CallInTreatment);
                    if (s_rand.Next(0, 5) == 0)
                    {
                        lock (AdminManager.BlMutex) //stage 7
                            CallManager.CompleteCall(v.Id, assignment.Id);
                    }
                    else if (s_rand.Next(0, 10) == 5)
                    {
                        lock (AdminManager.BlMutex) //stage 7
                            CallManager.CancelCall(v.Id, assignment.Id);
                    }
                }
                else
                {
                    //if (s_rand.Next(0, 5) == 0)
                    //{
                    //    lock (AdminManager.BlMutex)
                    //        s_list = CallManager.GetOpenCallsForVolunteer(v.Id, null, null).ToList();
                    //    lock (AdminManager.BlMutex)
                    //        CallManager.AssignCall(v.Id, s_list[s_rand.Next(0, s_list.Count)].Id);
                    //}
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}

