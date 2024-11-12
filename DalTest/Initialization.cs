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

    // the following arrays are made by AI (copilot)
    private static readonly string[] Names = new[]
    {
        "David Cohen", "Yossi Levi", "Miriam Katz", "Rivka Goldberg", "Moshe Ben-David",
        "Sara Peretz", "Yaakov Shapiro", "Rachel Mizrahi", "Shlomo Rosen", "Esther Friedman"
    };

    private static readonly string[] Emails = new[]
    {
        "david.cohen@example.com", "yossi.levi@example.com", "miriam.katz@example.com",
        "rivka.goldberg@example.com", "moshe.ben-david@example.com", "sara.peretz@example.com",
        "yaakov.shapiro@example.com", "rachel.mizrahi@example.com", "shlomo.rosen@example.com",
        "esther.friedman@example.com"
    };

    private static readonly string[] PhoneNumbers = new[]
    {
        "+972-50-1234567", "+972-52-2345678", "+972-54-3456789", "+972-55-4567890",
        "+972-56-5678901", "+972-57-6789012", "+972-58-7890123", "+972-59-8901234",
        "+972-50-9012345", "+972-52-0123456"
    };
    private static void _createVolunteer()
    {
        foreach (var name in Names) 
        {
            int id;
            bool? isActive = s_rand.Next(0, 2) == 1;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalVolunteer!.Read(id) != null);


        }
    }

    private static void _createAssignment()
    {
        // Implementation for creating assignments
    }

    private static void _createCall()
    {
        // Implementation for creating calls
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

        Console.WriteLine("Initializing Assignments list ...");
        _createAssignment();

        Console.WriteLine("Initializing Calls list ...");
        _createCall();

        Console.WriteLine("Initializing Volunteers list ...");
        _createVolunteer();
    }
}
