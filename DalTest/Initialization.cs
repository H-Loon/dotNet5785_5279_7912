using DalApi;

namespace DalTest;
public static class Initialization
{
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_Config;

    private static readonly Random s_rand = new();
}
