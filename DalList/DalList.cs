using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;

// Lazy singelton and Thread Safe
sealed internal class DalList : IDal
{
    private DalList() { }
    public static IDal Instance => Nested.Instance;
    private static class Nested
    {
        internal static readonly DalList Instance = new DalList();
    }
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig Config { get; }= new ConfigImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}