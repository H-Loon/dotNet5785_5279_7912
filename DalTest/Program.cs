namespace DalTest;

using Dal;
using DalApi;
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

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Volunteer Menu");
        Console.WriteLine("2. Call Menu");
        Console.WriteLine("3. Assignment Menu");
        Console.WriteLine("0. Exit");
    }

    private void DisplayEntityMenu(string entityName) 
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine($"1. Add {entityName}");
        Console.WriteLine($"2. Delete {entityName}");
        Console.WriteLine($"3. Delete all {entityName}s");
        Console.WriteLine($"4. Read {entityName}");
        Console.WriteLine($"5. Read all {entityName}s");
        Console.WriteLine($"6. Update {entityName}");
        Console.WriteLine("0. Exit");
    }

    private static void VolunteerMenu()
    {
        VolunteerMenu choice;
        do
        {
            choice = (VolunteerMenu)Console.Read();
            switch (choice)
            {
                case AddVolunteer:
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







    private void CMenu()
    {
       CallMenu choice;
        do
        {
          
            DisplayEntityMenu("Call");
            choice = (CallMenu)Console.Read();
            switch (choice)
            {
                case CallMenu.AddCall:
                    AddCall();
                    break;
                case CallMenu.DeleteVolunteer:
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
                    break;
            }
        } while (choice);
    }

    static private void AddCall()
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

        Console.WriteLine("Enter the lagitude");
        double callLagitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the longitude");
        double callLongitude = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the description");
        string callDescription = Console.ReadLine()!;

        /*Console.WriteLine("Enter the maximum time");
        DateTime callMaxTime = (DateTime)string.Parse(Console.ReadLine());*/

        Call caller = new Call(callType,callAdress,callLagitude,callLongitude,callDescription).s_dalCall.Create(Call);


    }

}

