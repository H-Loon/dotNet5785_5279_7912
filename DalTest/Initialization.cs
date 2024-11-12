using DalApi;
using DO;

namespace DalTest;
public static class Initialization
{
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();

    private static void _createAssignment()
    {
        
    }

    private static void _createCall()
    {

    }

    private static void _createVolunteer()
    {

    }

    public static void Do(IAssignment? dalAssignment, ICall? dalCall, IVolunteer? dalVolunteer, IConfig? dalConfig)
    {
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalAssignment.DeleteAll();
        s_dalCall.DeleteAll();

        Console.WriteLine("Initializing Students list ...");
        _createAssignment();
        _createCall();
        _createVolunteer();
    }
}
