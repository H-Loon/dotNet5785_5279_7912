namespace DalTest;

using Dal;
using DalApi;
using DO;

/// <summary>
/// Main program class for the DAL test application.
/// </summary>
internal class Program
{
    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXml(); //stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4


    #region Enum's
    private enum Menu
    {
        Exit,
        VolunteerMenu,
        CallMenu,
        AssignmentMenu,
        Initialize,
        ResetAllData,
        ReadAllData
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

    #endregion

    #region Main
    /// <summary>
    /// Main entry point of the program.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    static void Main(string[] args)
    {
        MainMenu();
    }

    #endregion

    #region Display's menu and entity menu
    /// <summary>
    /// Displays the main menu options.
    /// </summary>
    private static void DisplayMenu()
    {
        Console.WriteLine("\nChoose an option:");
        Console.WriteLine(" 1. Volunteer Menu");
        Console.WriteLine(" 2. Call Menu");
        Console.WriteLine(" 3. Assignment Menu");
        Console.WriteLine(" 4. Initialize");
        Console.WriteLine(" 5. Reset all data");
        Console.WriteLine(" 6. Read all data");
        Console.WriteLine(" 0. Exit\n");
    }

    /// <summary>
    /// Displays the entity-specific menu options.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
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

    #endregion

    #region Main menu and ReadAllData
    /// <summary>
    /// Main menu loop.
    /// </summary>
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
                    //Initialization.Do(s_dal); //stage 2
                    Initialization.Do(); //stage 4
                    break;
                case Menu.ResetAllData:
                    s_dal!.ResetDB();
                    Console.WriteLine("\nData has successfully been reseted");
                    break;
                case Menu.ReadAllData:
                    ReadAllData();
                    break;
            }
        } while (choice != Menu.Exit);
    }

    /// <summary>
    /// Reads all data from the database.
    /// </summary>
    private static void ReadAllData()
    {
        ReadAllVolunteers();
        Console.WriteLine();
        ReadAllCall();
        Console.WriteLine();
        ReadAllAssignments();
        Console.WriteLine();
    }

    #endregion

    #region Volunteer menu

    /// <summary>
    /// Volunteer menu loop.
    /// </summary>
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

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    private static void AddVolunteer()
    {
        try
        {
            s_dal!.Volunteer.Create(VolunteerFields("Add"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a volunteer by ID.
    /// </summary>
    private static void DeleteVolunteer()
    {
        // AI for the condition in if statement
        Console.Write("Enter volunteer's ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {
                Console.Write($"Are you sure you want to delete the volunteer ID={id}? (y/n): ");
                if (Console.ReadLine() == "n")
                    return;
                s_dal!.Volunteer.Delete(id);
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

    /// <summary>
    /// Deletes all volunteers.
    /// </summary>
    private static void DeleteAllVolunteers()
    {
        try {
            Console.Write("Are you sure you want to delete all the volunteers? (y/n): ");
            if (Console.ReadLine() == "n")
                return;
            s_dal!.Volunteer.DeleteAll();
            Console.WriteLine("All volunteers deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Reads a volunteer by ID.
    /// </summary>
    private static void ReadVolunteer()
    {
        try
        {
            Console.Write("Enter the volunteer ID you want to read or 0 to exit: ");
            int id = int.Parse(Console.ReadLine()!);
            if (id == 0)
                return;
            Volunteer? volunteer = s_dal!.Volunteer.Read(id) ?? throw new DalNotExistException($"Volunteer with Id = {id} does'nt exist");
            Console.WriteLine(volunteer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Reads all volunteers.
    /// </summary>
    private static void ReadAllVolunteers()
    {
        try {
            var volunteers = s_dal!.Volunteer.ReadAll();

            if (volunteers.Count() is 0)
            {
                Console.WriteLine("No volunteers found.");
                return;
            }

            foreach (var volunteer in volunteers)
            {
                Console.WriteLine(volunteer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Updates a volunteer by ID.
    /// </summary>
    private static void UpdateVolunteer()
    {
        Console.Write("Enter the volunteer ID you want to update or 0 to exit: ");
        int id = int.Parse(Console.ReadLine()!);
        if (id == 0)
            return;
        try
        {
            Console.WriteLine("Enter the new values for the volunteer:");
            s_dal!.Volunteer.Update(VolunteerFields("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Collects volunteer fields from the user.
    /// </summary>
    /// <param name="mod">The mode (Add or Update).</param>
    /// <param name="id">The ID of the volunteer (for Update mode).</param>
    /// <returns>A Volunteer object with the collected fields.</returns>
    private static Volunteer VolunteerFields(string mod, int id = -1)
    {
        string name, phoneNumber, email, adresse;
        RoleType role;
        bool isActive;
        double? maxDistance;

        if (mod == "Add")
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

        Console.Write("Enter volunteer's address: ");
        adresse = Console.ReadLine()!;

        Console.WriteLine("Enter volunteer's role:\n 1. Volunteer\n 2. Admin");
        role = (RoleType)int.Parse(Console.ReadLine()!);

        Console.Write("Is the volunteer active? y/n: ");
        isActive = (bool)(Console.ReadLine() == "n" ? false : true);

        Console.Write("Enter volunteer's max range: ");
        maxDistance = double.Parse(Console.ReadLine()!);

        return new Volunteer(id, name, phoneNumber, email, adresse, Role: role, IsActive: isActive, MaxDistance: maxDistance);
    }

    #endregion

    #region Call menu
    /// <summary>
    /// Call menu loop.
    /// </summary>
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

    /// <summary>
    /// Adds a new call.
    /// </summary>
    private static void AddCall()
    {
        s_dal!.Call.Create(CallFields("Add"));
    }

    /// <summary>
    /// Deletes a call by ID.
    /// </summary>
    private static void DeleteCall()
    {
        try
        {
            Console.WriteLine("Enter the Id of the Call to delete ");
            int id = int.Parse(Console.ReadLine()!);
            s_dal!.Call.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Deletes all calls.
    /// </summary>
    private static void DeleteAllCall()
    {
        try
        {
            s_dal!.Call.DeleteAll();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Reads a call by ID.
    /// </summary>
    private static void ReadCall()
    {    
        try
        {
            Console.WriteLine("Enter the id of the Call");
            int id = int.Parse(Console.ReadLine()!);
            Console.WriteLine(s_dal!.Call.Read(id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Reads all calls.
    /// </summary>
    private static void ReadAllCall()
    {
        try
        {
            var calls = s_dal!.Call.ReadAll();

            if (calls.Count() is 0)
            {
                Console.WriteLine("No calls found.");
                return;
            }

            foreach (var call in calls)
            {
                Console.WriteLine(call);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Updates a call by ID.
    /// </summary>
    private static void UpdateCall()
    {
        int id;
        Console.Write("Enter the assignment ID you want to update or 0 to exit: ");
        id = int.Parse(Console.ReadLine()!);

        if (id == 0)
            return;

        try
        {
            s_dal!.Call.Update(CallFields("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Collects call fields from the user.
    /// </summary>
    /// <param name="mod">The mode (Add or Update).</param>
    /// <param name="id">The ID of the call (for Update mode).</param>
    /// <returns>A Call object with the collected fields.</returns>
    private static Call CallFields(string mod, int id = -1)
    {
        CallType callType;
        string callAddress;
        double callLatitude, callLongitude;
        string? callDescription;
        DateTime? callMaxTime;

        if (mod == "Update") // Update
        {
            Call call = s_dal!.Call.Read(id)!;
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
            Console.Write("Do you want to change the address of the call? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New call's address: ");
                callAddress = Console.ReadLine()!;
            }
            Console.Write("Do you want to change the call's latitude? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New call's latitude: ");
                callLatitude = double.Parse(Console.ReadLine()!);
            }
            Console.Write("Do you want to change the call's longitude? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New call's longitude: ");
                callLongitude = double.Parse(Console.ReadLine()!);
            }
            Console.Write("Do you want to change the description of the call? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New call's description: ");
                callDescription = Console.ReadLine();
            }
            Console.Write("Do you want to change the max time of the call? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter New call's max time (DD/MM/YYYY): ");
                callMaxTime = DateTime.Parse(Console.ReadLine()!);
            }

            return new Call
            {
                Id = call.Id,
                Type = callType,
                Address = callAddress,
                Latitude = callLatitude,
                Longitude = callLongitude,
                StartTime = call.StartTime,
                Description = callDescription,
                MaxTime = callMaxTime
            };
        }
        else // Add
        {
            Console.WriteLine("Enter the type of the Call:");
            Console.WriteLine("0. Home Bot Issue,");
            Console.WriteLine("1. Teleporter Blocked On MachonLev,");
            Console.WriteLine("2. My Dishwasher Is In Depression,");
            Console.WriteLine("3. My Time Travel Machine Is Lazy,");
            Console.WriteLine("4. Other");
            callType = (CallType)int.Parse(Console.ReadLine()!);

            Console.Write("Enter the address of the Call: ");
            callAddress = Console.ReadLine()!;

            //Console.Write("Enter the latitude of the Call: ");
            //callLatitude = double.Parse(Console.ReadLine()!);

            //Console.Write("Enter the longitude of the Call: ");
            //callLongitude = double.Parse(Console.ReadLine()!);

            Console.Write("Enter the description of the Call: ");
            callDescription = Console.ReadLine();

            Console.Write("Enter the max time of the Call (DD/MM/YYYY): ");
            callMaxTime = DateTime.Parse(Console.ReadLine()!);

            Call caller = new Call(0, callType, callAddress, 0, 0, DateTime.Now, callDescription, callMaxTime);
            return caller;
        }
    }

    #endregion

    #region Assignment menu
    /// <summary>
    /// Assignment menu loop.
    /// </summary>
    private static void AMenu()
    {
        AssignmentMenu choice;
        do
        {
            DisplayEntityMenu("Assignment");
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

    /// <summary>
    /// Adds a new assignment.
    /// </summary>
    private static void AddAssignment()
    {
        s_dal!.Assignment.Create(AssignmentFields("Add"));
    }

    /// <summary>
    /// Deletes an assignment by ID.
    /// </summary>
    private static void DeleteAssignment()
    {
        try
        {
            Console.Write("Enter the assignment ID you want to delete or 0 to exit: ");
            int id = int.Parse(Console.ReadLine()!);

            if (id == 0)
                return;

            Console.Write($"Are you sure you want to delete the assignment ID={id}? (y/n): ");
            if (Console.ReadLine() == "n")
                return;

            s_dal!.Assignment.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Deletes all assignments.
    /// </summary>
    private static void DeleteAllAssignments()
    {
        try
        {
            Console.Write("Are you sure you want to delete all the assignments? (y/n): ");
            if (Console.ReadLine() == "n")
                return;
            s_dal!.Assignment.DeleteAll();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Reads an assignment by ID.
    /// </summary>
    private static void ReadAssignment()
    {
        try
        {
            Console.Write("Enter the assignment ID you want to read or 0 to exit: ");
            int id = int.Parse(Console.ReadLine()!);
            if (id == 0)
                return;
            Console.WriteLine(s_dal!.Assignment.Read(id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Reads all assignments.
    /// </summary>
    private static void ReadAllAssignments()
    {
        try
        {
            var assignments = s_dal!.Assignment.ReadAll();

            if (assignments.Count() is 0)
            {
                Console.WriteLine("No assignments found.");
                return;
            }

            foreach (var assignment in assignments)
            {
                Console.WriteLine(assignment);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Updates an assignment by ID.
    /// </summary>
    private static void UpdateAssignment()
    {
        try
        {
            int id;
            Console.Write("Enter the assignment ID you want to update or 0 to exit: ");
            id = int.Parse(Console.ReadLine()!);

            if (id == 0)
                return;
            s_dal!.Assignment.Update(AssignmentFields("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Collects assignment fields from the user.
    /// </summary>
    /// <param name="mod">The mode (Add or Update).</param>
    /// <param name="id">The ID of the assignment (for Update mode).</param>
    /// <returns>An Assignment object with the collected fields.</returns>
    private static Assignment AssignmentFields(string mod, int id = -1)
    {
        int callId, volunteerId;
        DateTime? endDate;
        AssignmentEndReason? endReason;

        if (mod == "Update") // Update
        {
            Assignment assignment = s_dal!.Assignment.Read(id)!;
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

            //Console.WriteLine("Enter the end date (DD/MM/YYYY): ");
            //endDate = DateTime.Parse(Console.ReadLine()!);

            return new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartDate = DateTime.Now,
                //EndDate = endDate
            };
        }
    }

    #endregion
}