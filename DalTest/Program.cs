using Dal;
using DalApi;
using System.Linq.Expressions;

namespace DalTest
{
    internal class Program
    {
        private static IConfig? s_dalConfig = new ConfigImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();


    static void Main(string[] args)
        {
  
        }