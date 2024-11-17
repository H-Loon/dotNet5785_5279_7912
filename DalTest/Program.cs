namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Diagnostics;
using System.Transactions;

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
            s_dalVolunteer!.Create(VolunteerField("Add"));
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
            s_dalVolunteer!.Update(VolunteerField("Update", id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static Volunteer VolunteerField(string mod, int id = -1)
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
    private void CMenu()
    {
       CallMenu choice;
        do
        {
            DisplayEntityMenu("Call");
            choice = (CallMenu)int.Parse(Console.ReadLine()!);
            switch (choice)
            {
                case CallMenu.AddCall:
                    AddCall();
                    break;
                    /*case CallMenu.DeleteVolunteer:
                        DeleteVolunteer();
                        break;
                    case CallMenu.DeleteCAll:
                        DeleteAllVolunteers();
                        break;
                    case CallMenu.ReadCall:
                        ReadCall();
                        break;
                    case CallMenu.ReadAllCall:
                        ReadAllVolunteers();
                        break;
                    case CallMenu.UpdateCall:
                        UpdateCall();
                        break;*/
                }
            } while (choice != CallMenu.Exit) ;
        }

    private static void AddCall()
    {
        Console.WriteLine(@"Enter the type of the Call:
                            1:HomeBotIssue,
                            2:TeleporterBlockedOnMachonLev,
                            3:MyDishwasherIsInDepression,
                            4:MyTimeTravelMachineIsLazy,
                            5:Other");
       
        CallType callType = (CallType)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the adress of the Call");
        string callAdress = Console.ReadLine()!;

        /*Console.WriteLine("Enter the lagitude");
        double callLagitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the longitude");
        double callLongitude = double.Parse(Console.ReadLine()!);*/

        Console.WriteLine("Enter the description");
        string callDescription = Console.ReadLine()!;

            /*Console.WriteLine("Enter the maximum time");
            DateTime callMaxTime = (DateTime)string.Parse(Console.ReadLine());*/

            Call caller = new Call(callType, callAdress, callDescription);
            s_dalCall!.Create(Call);


    }

}

