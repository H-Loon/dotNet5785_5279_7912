namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Diagnostics;
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
                //case Menu.CallMenu:
                //    CMenu();
                //    break;
                //case Menu.AssignmentMenu:
                //    AMenu();
                //    break;
            }
        } while (choice != Menu.Exit);
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
                //case VolunteerMenu.DeleteVolunteer:
                //    DeleteVolunteer();
                //    break;
                //case VolunteerMenu.DeleteAllVolunteers:
                //    DeleteAllVolunteers();
                //    break;
                //case VolunteerMenu.ReadVolunteer:
                //    ReadVolunteer();
                //    break;
                //case VolunteerMenu.ReadAllVolunteers:
                //    ReadAllVolunteers();
                //    break;
                //case VolunteerMenu.UpdateVolunteer:
                //    UpdateVolunteer();
                //    break;
            }
        } while (choice != VolunteerMenu.Exit);
    }

    private static void AddVolunteer() {
        int id;
        string name, phoneNumber, email, adresse;
        RoleType role;
        bool isActive;
        double? maxDistance;

        Console.WriteLine("Enter volunteer's ID: ");
        id = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter volunteer's name: ");
        name = Console.ReadLine()!;

        Console.WriteLine("Enter volunteer's phone number: ");
        phoneNumber = Console.ReadLine()!;

        Console.WriteLine("Enter volunteer's email: ");
        email = Console.ReadLine()!;

        Console.WriteLine("Enter volunteer's adresse: ");
        adresse = Console.ReadLine()!;

        Console.WriteLine("Enter volunteer's role:\n 1. Volunteer\n 2. Admin");
        role = (RoleType)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Is the volunteer active? y/n: ");
        isActive = (bool)(Console.ReadLine() == "n" ? false : true);

        Console.WriteLine("Enter volunteer's max range: ");
        maxDistance = double.Parse(Console.ReadLine()!);

        Volunteer volunteer = new Volunteer(id, name, phoneNumber, email, adresse, Role: role, IsActive: isActive, MaxDistance: maxDistance);
        s_dalVolunteer!.Create(volunteer);
    }
    private void CMenu()
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
            } while (choice != CallMenu.Exit) ;
        }
        catch (Exception ex) {
            Console.WriteLine(blabla);
        

    private static void AddCall()
    {
        Console.WriteLine("Enter the type of the Call:");
        Console.WriteLine("1. Home Bot Issue,");
        Console.WriteLine("2. Teleporter Blocked On MachonLev,");
        Console.WriteLine("3. My Dishwasher Is In Depression,");
        Console.WriteLine("4. My Time Travel Machine Is Lazy,");
        Console.WriteLine("5. Other");
       
        CallType callType = (CallType)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the adress of the Call");
        string callAdress = Console.ReadLine()!;

        /*Console.WriteLine("Enter the lagitude");
        double callLagitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the longitude");
        double callLongitude = double.Parse(Console.ReadLine()!);*/

        Console.WriteLine("Enter the description");
        string callDescription = Console.ReadLine()!;

        Console.WriteLine("Enter the end date");
        DateTime? callMaxTime = DateTime.Parse(Console.ReadLine());

        Call caller = new Call(0,callType, callAdress,0,0,DateTime.Now,callDescription,callMaxTime);
        s_dalCall!.Create(caller);
        
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
        Console.WriteLine("Enter the Id to update");
        int id = int.Parse(Console.ReadLine()!);
        Update(s_dalCall);
    }


}

