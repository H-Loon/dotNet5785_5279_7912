using System.Text;

namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    #region Enums
    private enum MainDisplayEnum
    {
        Exit,
        VolunteerMenu,
        CallMenu,
        AdminMenu
    }
    private enum VolunteerDisplayEnum
    {
        Back,
        Login,
        AddVolunteer,
        DeleteVolunteer,
        GetVolunteer,
        GetVolunteerInList,
        UpdateVolunteer
    }
    private enum CallDisplayEnum
    {
        Back,
        GetCallsQuantities,
        GetCallsInList,
        GetCall,
        UpdateCall,
        DeleteCall,
        AddCall,
        GetClosedCallByVolunteer,
        GetOpenCallForVolunteer,
        CompleteCall,
        CancelCall,
        AssignCall
    }
    private enum AdminDisplayEnum
    {
        Back,
        ForwardClock,
        GetConfigClock,
        GetRiskRange,
        UpdateRiskRange,
        InitDB,
        ResetDB
    }
    private enum StringTypeEnum
    {
        Id,
        Name,
        Phone
    }
    private enum ACCEnum
    {
        Assign,
        Complete,
        Cancel
    }
    #endregion

    #region Main and main display
    static void Main(string[] args)
    {
        MainDisplay();
    }
    private static void MainDisplay()
    {
        MainDisplayEnum choice;
        do
        {
            Console.Clear();

            Console.WriteLine("Main Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Volunteer Menu");
            Console.WriteLine("2. Call Menu");
            Console.WriteLine("3. Admin Menu");
            Console.Write("Please select an option: ");

            while (!Enum.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(MainDisplayEnum), choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 3.");
                Console.Write("Please select an option: ");
            }

            Console.Clear();

            switch (choice)
            {
                case MainDisplayEnum.VolunteerMenu:
                    VolunteerDisplay();
                    break;
                case MainDisplayEnum.CallMenu:
                    CallDisplay();
                    break;
                case MainDisplayEnum.AdminMenu:
                    AdminDisplay();
                    break;
            }
        } while (choice != MainDisplayEnum.Exit);
    }
    #endregion

    #region Admin display and methods
    /// <summary>
    /// Displays the Admin menu and handles user choice for various admin operations.
    /// </summary>
    private static void AdminDisplay()
    {
        AdminDisplayEnum choice;
        do
        {
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. Forward Clock");
            Console.WriteLine("2. Get Config Clock");
            Console.WriteLine("3. Get Risk Range");
            Console.WriteLine("4. Update Risk Range");
            Console.WriteLine("5. Init DB");
            Console.WriteLine("6. Reset DB");
            Console.Write("Please select an option: ");

            while (!Enum.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(AdminDisplayEnum), choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 5.");
                Console.Write("Please select an option: ");
            }

            //Console.Clear();
            switch (choice)
            {
                case AdminDisplayEnum.ForwardClock:
                    ForwardClock();
                    break;
                case AdminDisplayEnum.GetConfigClock:
                    Console.WriteLine(s_bl.Admin.GetConfigClock());
                    break;
                case AdminDisplayEnum.GetRiskRange:
                    Console.WriteLine(s_bl.Admin.GetRiskRange());
                    break;
                case AdminDisplayEnum.UpdateRiskRange:
                    UpdateRiskRange();
                    break;
                case AdminDisplayEnum.InitDB:
                    InitDB();
                    break;
                case AdminDisplayEnum.ResetDB:
                    ResetDB();
                    break;
            }
        } while (choice != AdminDisplayEnum.Back);
    }

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    private static void ResetDB()
    {
        try
        {
            s_bl.Admin.ResetDB();
            Console.WriteLine("Database reset successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Initializes the database.
    /// </summary>
    private static void InitDB()
    {
        try
        {
            s_bl.Admin.InitDB();
            Console.WriteLine("Database initialized successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Updates the risk range with a new value provided by the user.
    /// </summary>
    private static void UpdateRiskRange()
    {
        try
        {
            Console.Write("Enter the new risk range in hours: ");
            if (int.TryParse(Console.ReadLine(), out int hours))
            {
                s_bl.Admin.UpdateRiskRange(new TimeSpan(hours, 0, 0));
                Console.WriteLine("Risk range updated successfully.");
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Forwards the system clock by a specified value and time unit.
    /// </summary>
    private static void ForwardClock()
    {
        try
        {
            Console.Write("Enter the value to forward the clock by: ");
            if (int.TryParse(Console.ReadLine(), out int value))
            {
                Console.WriteLine("0. Seconds");
                Console.WriteLine("1. Minutes");
                Console.WriteLine("2. Hours");
                Console.WriteLine("3. Days");
                Console.WriteLine("4. Weeks");
                Console.WriteLine("5. Months");
                Console.WriteLine("6. Years");
                Console.Write("Please select a time unit: ");
                if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit timeUnit))
                {
                    s_bl.Admin.ForwardClock(value, timeUnit);
                    Console.WriteLine("Clock forwarded successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid time unit.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    #endregion

    #region Volunteer display and methods
    private static void VolunteerDisplay()
    {
        VolunteerDisplayEnum choice;
        do
        {
            Console.WriteLine("Volunteer Menu:");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Add Volunteer");
            Console.WriteLine("3. Delete Volunteer");
            Console.WriteLine("4. Get Volunteer");
            Console.WriteLine("5. Get Volunteer In List");
            Console.WriteLine("6. Update Volunteer");
            Console.Write("Please select an option: ");

            while (!Enum.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(VolunteerDisplayEnum), choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 5.");
                Console.Write("Please select an option: ");
            }

            //Console.Clear();
            switch (choice)
            {
                case VolunteerDisplayEnum.Login:
                    Login();
                    break;
                case VolunteerDisplayEnum.AddVolunteer:
                    AddVolunteer();
                    break;
                case VolunteerDisplayEnum.DeleteVolunteer:
                    DeleteVolunteer();
                    break;
                case VolunteerDisplayEnum.GetVolunteer:
                    GetVolunteer();
                    break;
                case VolunteerDisplayEnum.GetVolunteerInList:
                    GetVolunteerInList();
                    break;
                case VolunteerDisplayEnum.UpdateVolunteer:
                    UpdateVolunteer();
                    break;
            }
        } while (choice != VolunteerDisplayEnum.Back);
    }

    private static void UpdateVolunteer()
    {
        try
        {
            int active;
            int id = int.Parse(StringCheck(StringTypeEnum.Id));
            string name = StringCheck(StringTypeEnum.Name);

            Console.WriteLine("Is the volunteer Active?:");
            Console.WriteLine("0. No");
            Console.WriteLine("1. Yes");

            while (int.TryParse(Console.ReadLine(), out active) && active is < 0 or > 1)
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 2.");
                Console.Write("Please select an option: ");
            }

            BO.Volunteer volunteer = VolunteerFill();

            s_bl.Volunteer.UpdateVolunteer(id, new BO.Volunteer
            {
                Id = id,
                Name = name,
                Email = volunteer.Email,
                Password = volunteer.Password,
                Phone = volunteer.Phone,
                Address = volunteer.Address,
                IsActive = bool.Parse(active.ToString()),
                Role = volunteer.Role,
                MaxDistance = volunteer.MaxDistance,
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void GetVolunteerInList()
    {
        try
        {
            int choice;
            int? field;

            Console.WriteLine("What list of volunteer you need?");
            Console.WriteLine("0. All");
            Console.WriteLine("1. Active");
            Console.WriteLine("2. Non Active");
            Console.Write("Please select an option: ");

            while (int.TryParse(Console.ReadLine(), out choice) && choice is < 0 or > 2)
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 2.");
                Console.Write("Please select an option: ");
            }

            bool? active = choice switch
            {
                0 => null,
                1 => true,
                2 => false,
                _ => null
            };

            Console.WriteLine("What field you want to sort by?");
            Console.WriteLine("0. AssignmentId");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Active");
            Console.WriteLine("3. Completed Calls");
            Console.WriteLine("4. Canceled Calls");
            Console.WriteLine("5. Over Dated Calls");
            Console.WriteLine("6. Call In Treatment");
            Console.WriteLine("7. Current Call Type");
            Console.Write("Please select an option: ");
            field = int.Parse(Console.ReadLine());
            if (field is < 0 or > 7)
                field = null;

            foreach (var v in s_bl.Volunteer.GetVolunteerInList(active, field is null ? null :(BO.VolunteerInListField)field))
            {
                Console.WriteLine("" + v + '\n');
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void GetVolunteer()
    {
        try
        {
            int id = int.Parse(StringCheck(StringTypeEnum.Id));
            Console.WriteLine(s_bl.Volunteer.GetVolunteer(id));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void DeleteVolunteer()
    {
        try
        {
            Console.Write("Enter the volunteer's ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_bl.Volunteer.DeleteVolunteer(id);
                Console.WriteLine("Volunteer deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void AddVolunteer()
    {
        try
        {
            int id = int.Parse(StringCheck(StringTypeEnum.Id));
            
            string name = StringCheck(StringTypeEnum.Name);

            BO.Volunteer volunteer = VolunteerFill();

            s_bl.Volunteer.AddVolunteer(new BO.Volunteer
            {
                Id = id,
                Name = name,
                Email = volunteer.Email,
                Password = volunteer.Password,
                Phone = volunteer.Phone,
                Address = volunteer.Address,
                IsActive = true,
                Role = volunteer.Role,
                MaxDistance = volunteer.MaxDistance,
                CurrentCall = null,
                CompletedCalls = 0,
                CanceledCalls = 0,
                OverDatedCalls = 0
            });
            Console.WriteLine("\nVolunteer has successfuly been added!\n");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void Login()
    {
        try
        {
            string name = StringCheck(StringTypeEnum.Name);

            Console.Write("Enter your password or click enter if you don't have one: ");
            string? password = ReadPassword();

            if (string.IsNullOrWhiteSpace(password))
                password = null;

            Console.WriteLine(s_bl.Volunteer.Login(name, password));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static BO.Volunteer VolunteerFill()
    {
        try
        {
            Console.Write("Enter the volunteer's email: ");
            string email = Console.ReadLine()!;

            string password1="";
            string password2="";
            do
            {
                if (password1 != password2)
                    Console.WriteLine("Try again:");

                Console.Write("Enter the volunteer's password click enter to skip: ");
                password1 = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(password1))
                { 
                    Console.Write("Confirm the volunteer's password: ");
                    password2 = Console.ReadLine();
                }
            }
            while (password1 != password2);

            string phone = StringCheck(StringTypeEnum.Phone);

            Console.Write("Enter the volunteer's address or click enter to skip: ");
            string? address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                address = null;


            Console.Write("Enter the volunteer's max distance (Km) or click enter to skip: ");
            string maxDistanceInput = Console.ReadLine()!;
            double? maxDistance = null;
            if (!string.IsNullOrWhiteSpace(maxDistanceInput))
            {
                if (double.TryParse(maxDistanceInput, out double maxDistanceValue))
                {
                    maxDistance = maxDistanceValue;
                }
            }

            Console.WriteLine("Enter the volunteer's role: ");
            Console.WriteLine("0. Volunteer");
            Console.WriteLine("1. Admin");
            int role;
            while (!int.TryParse(Console.ReadLine(), out role) || (role != 0 && role != 1))
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 1.");
                Console.WriteLine("Enter the volunteer's role: ");
            }

            return new BO.Volunteer
            {
                Id = 0,
                Name = "",
                Email = email,
                Password = password2,
                Phone = phone,
                Address = address,
                IsActive = true,
                Role = (BO.BoRoleType)role,
                MaxDistance = maxDistance,
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("Please try again.");
            return VolunteerFill();
        }
    }


    private static string ReadPassword() // Ai helped
    {
        var password = new StringBuilder();
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(intercept: true);
            if (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace)
            {
                password.Append(keyInfo.KeyChar);
                Console.Write("*");
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
        } while (keyInfo.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password.ToString();
    }
    #endregion

    #region Call display and methods
    private static void CallDisplay()
    {
        CallDisplayEnum choice;
        do
        {
            Console.WriteLine("\nCall Menu:");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. Get Calls Quantities");
            Console.WriteLine("2. Get Calls In List");
            Console.WriteLine("3. Get Call");
            Console.WriteLine("4. Update Call");
            Console.WriteLine("5. Delete Call");
            Console.WriteLine("6. Add Call");
            Console.WriteLine("7. Get Closed Call By Volunteer");
            Console.WriteLine("8. Get Open Call For Volunteer");
            Console.WriteLine("9. Complete Call");
            Console.WriteLine("10. Cancel Call");
            Console.WriteLine("11. Assign Call");
            Console.Write("Please select an option: ");

            while (!Enum.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(typeof(CallDisplayEnum), choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 11.");
                Console.Write("Please select an option: ");
            }

            switch (choice)
            {
                case CallDisplayEnum.GetCallsQuantities:
                    GetCallsQuantities();
                    break;
                case CallDisplayEnum.GetCallsInList:
                    GetCallsInList();
                    break;
                case CallDisplayEnum.GetCall:
                    GetCall();
                    break;
                case CallDisplayEnum.UpdateCall:
                    UpdateCall();
                    break;
                case CallDisplayEnum.DeleteCall:
                    DeleteCall();
                    break;
                case CallDisplayEnum.AddCall:
                    AddCall();
                    break;
                case CallDisplayEnum.GetClosedCallByVolunteer:
                    GetClosedCallByVolunteer();
                    break;
                case CallDisplayEnum.GetOpenCallForVolunteer:
                    GetOpenCallForVolunteer();
                    break;
                case CallDisplayEnum.CompleteCall:
                    CompleteCall();
                    break;
                case CallDisplayEnum.CancelCall:
                    CancelCall();
                    break;
                case CallDisplayEnum.AssignCall:
                    AssignCall();
                    break;
            }
        } while (choice != CallDisplayEnum.Back);
    }

    private static void GetCallsQuantities()
    {
        try
        {
            int[] quantities = s_bl.Call.GetCallsQuantities();
            Console.WriteLine("Calls Quantities:");
            foreach (var status in Enum.GetValues(typeof(BO.BoCallStatus)))
            {
                Console.WriteLine($"{status}: {quantities[(int)status]}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void GetCallsInList()
    {
        try
        {
            BO.CallInListField? field1 = null, field2 = null;
            object? value1 = null;
            Console.WriteLine("What field you want to filtered by?");
            CallInListDisplay();

            int input;
            int.TryParse(Console.ReadLine(), out input);
            if (input >= 0 && input <= 7)
                field1 = (BO.CallInListField)input;
            else if (input == 9)
                field1 = null;
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }
            if (field1 is not null)
            {
                switch (field1)
                {
                    case BO.CallInListField.AssignmentId:
                        Console.WriteLine("Enter the AssignmentId: ");
                        int.TryParse(Console.ReadLine(), out int tempInt);
                        value1 = tempInt;
                        break;
                    case BO.CallInListField.CallId:
                        Console.WriteLine("Enter the CallId: ");
                        int.TryParse(Console.ReadLine(), out tempInt);
                        value1 = tempInt;
                        break;
                    case BO.CallInListField.CallType:
                        foreach (var callType in Enum.GetValues(typeof(DO.CallType))) // to not print the None value
                        {
                            Console.WriteLine($"{(int)callType}. {callType}");
                        }
                        Console.Write("Choose the CallType: ");
                        if (Enum.TryParse(Console.ReadLine(), out BO.BoCallType callTypeSelected1))
                            value1 = callTypeSelected1;
                        else
                            Console.WriteLine("Invalid call type.");
                        break;
                    case BO.CallInListField.StartTime:
                        Console.WriteLine("Enter the StartTime: ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime tempDateTime))
                            value1 = tempDateTime;
                        else
                            Console.WriteLine("Invalid date.");
                        break;
                    case BO.CallInListField.TimeLeft:
                        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan tempTimeSpan))
                            value1 = tempTimeSpan;
                        else
                            Console.WriteLine("Invalid date.");
                        break;
                    case BO.CallInListField.LastVolunteerName:
                        value1 = StringCheck(StringTypeEnum.Name);
                        break;
                    case BO.CallInListField.TimeOpen:
                        //Console.WriteLine("Choose between from or till the time open:");
                        //Console.WriteLine("0. From");
                        //Console.WriteLine("1. Till");
                        //Console.Write("Please select an option: ");
                        //if (int.TryParse(Console.ReadLine(), out int choice) && choice != 0 && choice != 1)
                        //{
                        //    Console.WriteLine("Invalid choice.");
                        //    break;
                        //}

                        Console.WriteLine("Enter the TimeOpen: ");
                        if (TimeSpan.TryParse(Console.ReadLine(), out tempTimeSpan))
                            value1 = tempTimeSpan;
                        else
                            Console.WriteLine("Invalid date.");
                        break;
                    case BO.CallInListField.CallStatus:
                        Console.WriteLine("Enter the CallStatus: ");
                        foreach (var callType in Enum.GetValues(typeof(BO.BoCallStatus)))
                        {
                            Console.WriteLine($"{(int)callType}. {callType}");
                        }
                        Console.Write("Choose the CallType: ");
                        if (Enum.TryParse(Console.ReadLine(), out BO.BoCallStatus callTypeSelected2))
                            value1 = callTypeSelected2;
                        else
                            Console.WriteLine("Invalid call type.");
                        break;
                    case BO.CallInListField.AssignCount:
                        Console.WriteLine("Enter the AssignCount: ");
                        int.TryParse(Console.ReadLine(), out tempInt);
                        value1 = tempInt;
                        break;
                }
            }

            Console.WriteLine("What field you want to sort by?");
            CallInListDisplay();

            int.TryParse(Console.ReadLine(), out input);
            if (input >= 0 && input <= 7)
                field2 = (BO.CallInListField)input;
            else if (input == 9)
                field2 = null;
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            if (field1 is null)
            {
                foreach (var call in s_bl.Call.GetCallsInList(null, null, field2))
                {
                    Console.WriteLine(call);
                }
            }
            else
            {
                foreach (var call in s_bl.Call.GetCallsInList(field1, value1, field2))
                {
                    Console.WriteLine(call);
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void CallInListDisplay()
    {
        Console.WriteLine("0. Assignment ID");
        Console.WriteLine("1. Call ID");
        Console.WriteLine("2. Call type");
        Console.WriteLine("3. Start time");
        Console.WriteLine("4. Time left");
        Console.WriteLine("5. Last volunteer name");
        Console.WriteLine("6. Time open");
        Console.WriteLine("7. Call status");
        Console.WriteLine("8. Assigned times");
        Console.WriteLine("9. None");
        Console.Write("Please select an option: ");
    }

    private static void GetCall()
    {
        try
        {
            Console.Write("Enter the call ID: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                var call = s_bl.Call.GetCall(callId);
                Console.WriteLine(call);
            }
            else
            {
                Console.WriteLine("Invalid call ID.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void UpdateCall()
    {
        try
        {
            Console.Write("Enter the call ID: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                var call = s_bl.Call.GetCall(callId);
                Console.WriteLine("Current Call Details: " + call);

                Console.Write("Enter new description (or press Enter to keep current): ");
                string? description = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(description))
                {
                    call.Description = description;
                }

                Console.Write("Enter new address (or press Enter to keep current): ");
                string? address = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(address))
                {
                    call.Address = address;
                }

                Console.Write("Enter new status (or press Enter to keep current): ");
                string? statusInput = Console.ReadLine();
                if (Enum.TryParse(statusInput, out BO.BoCallStatus status))
                {
                    call.Status = status;
                }

                s_bl.Call.UpdateCall(call);
                Console.WriteLine("Call updated successfully.");
            }
            else
            {
                Console.WriteLine("Invalid call ID.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void DeleteCall()
    {
        try
        {
            Console.Write("Enter the call ID: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                s_bl.Call.DeleteCall(callId);
                Console.WriteLine("Call deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid call ID.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void AddCall()
    {
        try
        {
            Console.WriteLine("Choose a call type2:");
            foreach (var callType in Enum.GetValues(typeof(BO.BoCallType)))
            {
                Console.WriteLine($"{(int)callType}. {callType}");
            }
            Console.Write("Please select a call type2: ");
            if (Enum.TryParse(Console.ReadLine(), out BO.BoCallType callTypeSelected))
            {
                Console.Write("Enter the call description: ");
                string description = Console.ReadLine()!;

                Console.Write("Enter the call address: ");
                string address = Console.ReadLine()!;

                Console.Write("Enter the call max time or click Enter to no limit: ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime maxTime))
                {
                    BO.Call newCall = new BO.Call
                    {
                        Id = 0,
                        Description = description,
                        Address = address,
                        CallType = callTypeSelected,
                        Status = BO.BoCallStatus.Open,
                        StartTime = DateTime.Now,
                        MaxTime = maxTime
                    };

                    s_bl.Call.AddCall(newCall);
                    Console.WriteLine("Call added successfully.");
                }
            }
            else
            {
                Console.WriteLine("Invalid call type2.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void GetClosedCallByVolunteer()
    {
        try
        {
            // Implement logic to get closed call by volunteer
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void GetOpenCallForVolunteer()
    {
        try
        {
            // Implement logic to get open call for volunteer
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void CompleteCall()
    {
        try
        {
            ACCCall(ACCEnum.Complete);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void CancelCall()
    {
        try
        {
            ACCCall(ACCEnum.Cancel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void AssignCall()
    {
        try
        {
            ACCCall(ACCEnum.Assign);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    private static void ACCCall(ACCEnum action)
    {
        string type1 = action switch
        {
            ACCEnum.Assign => "volunteer",
            ACCEnum.Complete => "volunteer",
            ACCEnum.Cancel => "canceler",
            _ => ""
        };
        string type2 = action switch
        {
            ACCEnum.Assign => "call",
            ACCEnum.Complete => "assignment",
            ACCEnum.Cancel => "assignment",
            _ => ""
        };
        Console.Write($"Enter the {type1} ID: ");
        if (int.TryParse(Console.ReadLine(), out int typeId1))
        {
            Console.Write($"Enter the {type2} ID: ");
            if (int.TryParse(Console.ReadLine(), out int typeId2))
            {
                switch (action)
                {
                    case ACCEnum.Assign:
                        s_bl.Call.AssignCall(typeId1, typeId2);
                        Console.WriteLine("Call assigned successfully.");
                        break;
                    case ACCEnum.Complete:
                        s_bl.Call.CompleteCall(typeId1, typeId2);
                        Console.WriteLine("Call completed successfully.");
                        break;
                    case ACCEnum.Cancel:
                        s_bl.Call.CancelCall(typeId1, typeId2);
                        Console.WriteLine("Call canceled successfully.");
                        break;
                    default:
                        Console.WriteLine("Invalid action.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Invalid {type2} ID.");
            }
        }
        else
        {
            Console.WriteLine($"Invalid {type1} ID.");
        }
    }
#endregion
    #region Helper methods
    private static string StringCheck(StringTypeEnum type)
    {
        string? str;
        do
        {
            Console.Write($"Enter the volunteer's {type}: ");
            str = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(str));
        return str;
    }
    #endregion
}


/*
213017833
Menahem Katzir
mkatzir@gmail.com
MashiahNow770!
MashiahNow770!
+972-54-1234567
770 Eastern Parkway, Brooklyn, NY 11213, USA
50
0
 */