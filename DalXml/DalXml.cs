using DalApi;
using System.Diagnostics;
namespace Dal;

// Lazy Singleton and Thread Safe
sealed internal class DalXml : IDal
{
    private DalXml() { }
    public static IDal Instance => Nested.Instance;
    private static class Nested
    { 
        internal static readonly IDal Instance = new DalXml();
    }
    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
