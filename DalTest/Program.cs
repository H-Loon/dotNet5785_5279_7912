namespace DalTest;

using Dal;
using DalApi;
using DO;

internal class Program
{
    private static IConfig? s_dalConfig = new ConfigImplementation();
    private static ICall? s_dalCall = new CallImplementation();
    private static IAssignment? s_dalAssignment = new AssignmentImplementation();
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();

    private enum Menu
    {
        Exit,
        VolunteerMenu,
        CallMenu,
        AssignmentMenu,
        Initialize,
    }
    private enum VolunteerMenu
    {
        Exit,
        AddVolunteer,
        DeleteVolunteer,
        DeleteAllVolunteers,
        ReadVolunteer,
        ReadAllVolunteers,
        UpdateVolunteer,
    }
    private enum CallMenu
    {
        Exit,
        AddCall,
        DeleteCall,
        DeleteAllCalls,
        ReadCall,
        ReadAllCalls,
        UpdateCall,
    }
    private enum AssignmentMenu
    {
        Exit,
        AddAssignment,
        DeleteAssignment,
        DeleteAllAssignments,
        ReadAssignment,
        ReadAllAssignments,
        UpdateAssignment,
    }

    static void Main(string[] args)
    {
        try
        {
            MainMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    private static void DisplayMenu()
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine(" 1. Volunteer Menu");
        Console.WriteLine(" 2. Call Menu");
        Console.WriteLine(" 3. Assignment Menu");
        Console.WriteLine(" 4. Initialize");
        Console.WriteLine(" 0. Exit\n");
    }

    private static void DisplayEntityMenu(string entityName) 
    {
        Console.WriteLine("\nChoose an option:");
        Console.WriteLine($" 1. Add {entityName}");
        Console.WriteLine($" 2. Delete {entityName}");
        Console.WriteLine($" 3. Delete all {entityName}s");
        Console.WriteLine($" 4. Read {entityName}");
        Console.WriteLine($" 5. Read all {entityName}s");
        Console.WriteLine($" 6. Update {entityName}");
        Console.WriteLine(" 0. Exit\n");
    }

    private static void MainMenu()
    {
        Menu choice;
        do
        {
            DisplayMenu();
            choice = (Menu)int.Parse(Console.ReadLine()!);
            switch (choice)
            {
                case Menu.VolunteerMenu:
                    VMenu();
                    break;
                case Menu.CallMenu:
                    CMenu();
                    break;
                case Menu.AssignmentMenu:
                    AMenu();
                    break;
                case Menu.Initialize:
                    Initialization.Do(s_dalAssignment,s_dalCall,s_dalVolunteer,s_dalConfig);
                    break;
            }
        } while (choice != Menu.Exit);
    }

    private static void AMenu()
    {
        AssignmentMenu choice;
        do
        {
            DisplayEntityMenu("Volunteer");
            choice = (AssignmentMenu)int.Parse(Console.ReadLine()!);
            switch (choice)
            {
                case AssignmentMenu.AddAssignment:
                    AddAssignment();
                    break;
                case AssignmentMenu.DeleteAssignment:
                    DeleteAssignment();
                    break;
                case AssignmentMenu.DeleteAllAssignments:
                    DeleteAllAssignments();
                    break;
                case AssignmentMenu.ReadAssignment:
                    ReadAssignment();
                    break;
                case AssignmentMenu.ReadAllAssignments:
                    ReadAllAssignments();
                    break;
                case AssignmentMenu.UpdateAssignment:
                    UpdateAssignment();
                    break;
            }
        } while (choice != AssignmentMenu.Exit);
    }

    private static void AddAssignment()
    {
        s_dalAssignment!.Create(AssignmentFields("Add"));
    }

    private static void DeleteAssignment()
    {
        try
        {
            Console.Write("Enter the assignment ID you want to delete or 0 to exit: ");
            int id = int.Parse(Console.ReadLine()!);

            if (id == 0)
                return;

            Console.Write($"Are you sur you want to delete the assignment ID={id}? (y/n): ");
            if (Console.ReadLine() == "n")
                return;

            s_dalAssignment!.Delete(int.Parse(Console.ReadLine()!));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void DeleteAllAssignments()
    {
        Console.Write("Are you sur you want to delete All the assignment's ? (y/n): ");
        if (Console.ReadLine() == "n")
            return;
        s_dalAssignment!.DeleteAll();
    }

    private static void ReadAssignment()
    {
        Console.Write("Enter the assignment ID you want to read or 0 to exit: ");
        int id = int.Parse(Console.ReadLine()!);
        if (id == 0)
            return;
        Console.WriteLine(s_dalAssignment!.Read(id));
    }

    private static void ReadAllAssignments()
    {
        Console.WriteLine(s_dalAssignment!.ReadAll());
    }

    private static void UpdateAssignment()
    {
        int id;
        Console.Write("Enter the assignment ID you want to update or 0 to exit: ");
        id = int.Parse(Console.ReadLine()!);

        if (id == 0)
            return;

        try
        {
            s_dalAssignment!.Update(AssignmentFields("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static Assignment AssignmentFields(string mod, int id = -1)
    {
        int callId, volunteerId;
        DateTime? endDate;
        AssignmentEndReason? endReason;

        if (mod == "Update") // Update
        {
            Assignment assignment = s_dalAssignment!.Read(id)!;
            callId = assignment.CallId;
            volunteerId = assignment.VolunteerId;
            endDate = assignment.EndDate;
            endReason = assignment.EndReason;

            Console.Write("Do you want to change the call's ID of the assignment? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New call's ID: ");
                callId = int.Parse(Console.ReadLine()!);
            }

            Console.Write("Do you want to change the volunteer's ID of the assignment? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New volunteer's ID: ");
                volunteerId = int.Parse(Console.ReadLine()!);
            }

            Console.Write("Do you want to change the status of the assignment? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.WriteLine("Enter the reason for ending the assignment:\n 1. Completed\n 2. Canceled by me\n 3. Canceled by admin\n 4. Over dated");
                endReason = (AssignmentEndReason)int.Parse(Console.ReadLine()!);
            }

            Console.Write("Do you want to change the end date of the assignment? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.WriteLine("Enter the New end date (DD/MM/YYYY): ");
                endDate = DateTime.Parse(Console.ReadLine()!);
            }

            return new Assignment
            {
                Id = assignment.Id,
                CallId = callId,
                VolunteerId = volunteerId,
                StartDate = assignment.StartDate,
                EndDate = endDate,
                EndReason = endReason
            };
        }
        else // Add
        {
            Console.Write("Enter call's ID: ");
            callId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter volunteer's ID: ");
            volunteerId = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Enter the end date (DD/MM/YYYY): ");
            endDate = DateTime.Parse(Console.ReadLine()!);

            return new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartDate = DateTime.Now,
                EndDate = endDate
            };
        }
    }

    private static void VMenu()
    {   
        VolunteerMenu choice;
        do
        {
            DisplayEntityMenu("Volunteer");
            choice = (VolunteerMenu)int.Parse(Console.ReadLine()!);
            switch (choice)
            {
                case VolunteerMenu.AddVolunteer:
                    AddVolunteer();
                    break;
                case VolunteerMenu.DeleteVolunteer:
                    DeleteVolunteer();
                    break;
                case VolunteerMenu.DeleteAllVolunteers:
                    DeleteAllVolunteers();
                    break;
                case VolunteerMenu.ReadVolunteer:
                    ReadVolunteer();
                    break;
                case VolunteerMenu.ReadAllVolunteers:
                    ReadAllVolunteers();
                    break;
                case VolunteerMenu.UpdateVolunteer:
                    UpdateVolunteer();
                    break;
            }
        } while (choice != VolunteerMenu.Exit);
    }

    private static void AddVolunteer()
    {
        try
        {
            s_dalVolunteer!.Create(VolunteerFields("Add"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void DeleteVolunteer()
    {
        // AI for the condition in if statement
        Console.Write("Enter volunteer's ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {
                Console.Write($"Are you sur you want to delete the volunteer ID={id}? (y/n): ");
                if (Console.ReadLine() == "n")
                    return;
                s_dalVolunteer!.Delete(id);
                Console.WriteLine("Volunteer deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Invalid ID. Please enter a valid number.");
        }
    }

    private static void DeleteAllVolunteers()
    {
        Console.Write("Are you sur you want to delete all the volunteers? (y/n): ");
        if (Console.ReadLine() == "n")
            return;
        s_dalVolunteer!.DeleteAll();
        Console.WriteLine("All volunteers deleted successfully.");
    }

    private static void ReadVolunteer()
    {
        Console.Write("Enter the volunteer ID you want to read or 0 to exit: ");
        int id = int.Parse(Console.ReadLine()!);
        if (id == 0)
            return;
        Console.WriteLine(s_dalVolunteer!.Read(id));
    }

    private static void ReadAllVolunteers()
    {
        Console.WriteLine(s_dalVolunteer!.ReadAll());
    }

    private static void UpdateVolunteer()
    {
        Console.Write("Enter the volunteer ID you want to update or 0 to exit: ");
        int id = int.Parse(Console.ReadLine()!);
        if (id == 0)
            return;
        try
        {
            Console.WriteLine("Enter the new values for the volunteer:");
            s_dalVolunteer!.Update(VolunteerFields("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static Volunteer VolunteerFields(string mod, int id = -1)
    {
        string name, phoneNumber, email, adresse;
        RoleType role;
        bool isActive;
        double? maxDistance;

        if(mod == "Add")
        {
            Console.Write("Enter volunteer's ID: ");
            id = int.Parse(Console.ReadLine()!);
        }

        Console.Write("Enter volunteer's name: ");
        name = Console.ReadLine()!;

        Console.Write("Enter volunteer's phone number: ");
        phoneNumber = Console.ReadLine()!;

        Console.Write("Enter volunteer's email: ");
        email = Console.ReadLine()!;

        Console.Write("Enter volunteer's adresse: ");
        adresse = Console.ReadLine()!;

        Console.WriteLine("Enter volunteer's role:\n 1. Volunteer\n 2. Admin");
        role = (RoleType)int.Parse(Console.ReadLine()!);

        Console.Write("Is the volunteer active? y/n: ");
        isActive = (bool)(Console.ReadLine() == "n" ? false : true);

        Console.Write("Enter volunteer's max range: ");
        maxDistance = double.Parse(Console.ReadLine()!);

        return new Volunteer(id, name, phoneNumber, email, adresse, Role: role, IsActive: isActive, MaxDistance: maxDistance);
    }
    private static void CMenu()
    {
       CallMenu choice;
        try
        {
            do
            {
                DisplayEntityMenu("Call");
                choice = (CallMenu)int.Parse(Console.ReadLine()!);
                switch (choice)
                {
                    case CallMenu.AddCall:
                        AddCall();
                        break;
                    case CallMenu.DeleteCall:
                        DeleteCall();
                        break;
                    case CallMenu.DeleteAllCalls:
                        DeleteAllCall();
                        break;
                    case CallMenu.ReadCall:
                        ReadCall();
                        break;
                    case CallMenu.ReadAllCalls:
                        ReadAllCall();
                        break;
                    case CallMenu.UpdateCall:
                        UpdateCall();
                        break;
                }
            } while (choice != CallMenu.Exit);
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex);
        }
    }

    private static void AddCall()
    {
        Console.WriteLine("Enter the type of the Call:");
        Console.WriteLine("1. Home Bot Issue,");
        Console.WriteLine("2. Teleporter Blocked On MachonLev,");
        Console.WriteLine("3. My Dishwasher Is In Depression,");
        Console.WriteLine("4. My Time Travel Machine Is Lazy,");
        Console.WriteLine("5. Other");

        s_dalAssignment!.Create(AssignmentFields("Add"));

        /*CallType callType = (CallType)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the adress of the Call");
        string callAdress = Console.ReadLine()!;

        /*Console.WriteLine("Enter the lagitude");
        double callLagitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the longitude");
        double callLongitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the description");
        string callDescription = Console.ReadLine()!;

        Console.WriteLine("Enter the end date");
        DateTime? callMaxTime = DateTime.Parse(Console.ReadLine()!);

        Call caller = new Call(0,callType, callAdress,0,0,DateTime.Now,callDescription,callMaxTime);
        s_dalCall!.Create(caller);
        */
    }
    private static void DeleteCall()
    {
        Console.WriteLine("Enter the Id of the Call to delete ");
        int id = int.Parse(Console.ReadLine()!) ;
        s_dalCall!.Delete(id);
    }
    private static void DeleteAllCall()
    {
        s_dalCall!.DeleteAll();
    }
    private static void ReadCall()
    {
        Console.WriteLine("Enter the id of the Call");
        int id = int.Parse(Console.ReadLine()!);
        Console.WriteLine(s_dalCall!.Read(id));
    }
    private static void ReadAllCall()
    {
        Console.WriteLine(s_dalCall!.ReadAll());
    }
    private static void UpdateCall()
    {
        int id;
        Console.Write("Enter the assignment ID you want to update or 0 to exit: ");
        id = int.Parse(Console.ReadLine()!);

        if (id == 0)
            return;

        try
        {
            s_dalAssignment!.Update(CallFields("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

private static Call CallFields(string mod, int id = -1)
{
    CallType callType;
    string callAddress;
    double callLatitude, callLongitude;
    string? callDescription;
    DateTime? callMaxTime;

    if (mod == "Update") // Update
    {
        Call call = s_dalCall!.Read(id)!;
        callType = call.Type;
        callAddress = call.Address;
        callLatitude = call.Latitude;
        callLongitude = call.Longitude;
        callDescription = call.Description; 
        callMaxTime = call.MaxTime;


        Console.Write("Do you want to change the type of the call? (y/n): ");
        if (Console.ReadLine() == "y")
        {
            callType = (CallType)int.Parse(Console.ReadLine()!);
        }
        Console.Write("Do you want to change the adress of the call? (y/n): ");
        if (Console.ReadLine() == "y")
        {
            Console.Write("Enter New call's Iadress: ");
            callAddress = Console.ReadLine()!;
        }
        Console.Write("Do you want to change the call's latitude ? (y/n): ");
        if (Console.ReadLine() == "y")
        {
            Console.Write("Enter New call's latitude: ");
            callLatitude = int.Parse(Console.ReadLine()!);
        }
        Console.Write("Do you want to change the call's longitude ? (y/n): ");
        if (Console.ReadLine() == "y")
        {
            Console.Write("Enter New call's longitude: ");
            callLongitude = int.Parse(Console.ReadLine()!);
        }
        Console.Write("Do you want to change the end date of the assignment? (y/n): ");
        if (Console.ReadLine() == "y")
        {
            Console.WriteLine("Enter the New end date (DD/MM/YYYY): ");
            callMaxTime = DateTime.Parse(Console.ReadLine()!);
        }
        return new Call
        {
            Id = call.Id,
            Type = callType,
            Address = callAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            StartTime = call.StartTime,
            MaxTime = callMaxTime,
        };

    }
    else // Add
    {
        Console.WriteLine("Enter the type of the Call:");
        Console.WriteLine("1. Home Bot Issue,");
        Console.WriteLine("2. Teleporter Blocked On MachonLev,");
        Console.WriteLine("3. My Dishwasher Is In Depression,");
        Console.WriteLine("4. My Time Travel Machine Is Lazy,");
        Console.WriteLine("5. Other");
        callType = (CallType)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the adress of the Call");
        string callAdress = Console.ReadLine()!;

        Console.WriteLine("Enter the description");
        callDescription = Console.ReadLine()!;

        Console.WriteLine("Enter the end date");
        callMaxTime = DateTime.Parse(Console.ReadLine()!);

        Call caller = new Call(0, callType, callAdress, 0, 0, DateTime.Now, callDescription, callMaxTime);
        s_dalCall!.Create(caller);
        return caller;
        /*return new Call
        {
            Type = callType,
            Address = callAdress,
            Description = callDescription,
            MaxTime = callMaxTime,
        };*/
    }
}
    }