namespace DalTest;

using Dal;
using DalApi;

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
}
