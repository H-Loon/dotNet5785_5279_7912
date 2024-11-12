using DalApi;
using DO;
using System.Net;
using System.Numerics;

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

    private static readonly List<string> PhoneNumbers = new()
    {
                "+972-50-1234567", "+972-52-2345678", "+972-54-3456789", "+972-55-4567890",
                "+972-56-5678901", "+972-57-6789012", "+972-58-7890123", "+972-59-8901234",
                "+972-50-9012345", "+972-52-0123456"
            };

    private static readonly List<string> Addresses = new()
    {
                "1 Rothschild Blvd, Tel Aviv",
                "2 Herzl St, Haifa",
                "3 Jabotinsky St, Ramat Gan",
                "4 Ben Yehuda St, Jerusalem",
                "5 Dizengoff St, Tel Aviv",
                "6 Weizmann St, Rehovot",
                "7 Begin Blvd, Beersheba",
                "8 Allenby St, Tel Aviv",
                "9 King George St, Jerusalem",
                "10 Bialik St, Ramat Gan"
            };

    private static readonly List<double> Latitudes = new()
    {
                32.065, 32.818, 32.083, 31.776, 32.075,
                31.894, 31.252, 32.067, 31.780, 32.082
            };

    private static readonly List<double> Longitudes = new()
    {
                34.774, 34.988, 34.814, 35.213, 34.774,
                34.811, 34.791, 34.770, 35.220, 34.814
            };

    private static void _createVolunteer()
    {
        for (int i = 0; i < Names.Length; i++)
        {
            int id;
            string phone, address;
            double latitude, longitude;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalVolunteer!.Read(id) != null);


            phone = PhoneNumbers[s_rand.Next(0, PhoneNumbers.Count-1)];
            PhoneNumbers.Remove(phone);

            int r = s_rand.Next(0, Addresses.Count-1);
            address = Addresses[r];
            latitude = Latitudes[r];
            longitude = Longitudes[r];
            Addresses.RemoveAt(r);
            Latitudes.RemoveAt(r);
            Longitudes.RemoveAt(r);

            s_dalVolunteer.Create(new Volunteer
            {
                Id = id,
                Name = Names[i],
                Phone = phone,
                Email = Emails[i],
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                IsActive = s_rand.Next(0, 1) == 1,
                MaxDistance = s_rand.Next(1, 100),
            });
        }
    }

    private static void _createAssignment()
    {
        foreach (var name in Names)
        {
            int id;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalAssignment.Read(id) != null);
            int callId;
            do
            {
                callId = s_rand.Next(100000, 999999);
            } while (s_dalCall.Read(callId) == null);
            int volunteerId;
            do
            {
                volunteerId = s_rand.Next(1000, 9999);
            } while (s_dalVolunteer.Read(volunteerId) != null);

            DateTime startTime = new DateTime(1995, 1, 1);
            DateTime endTime = startTime.AddDays(s_rand.Next((s_dalConfig.Clock - startTime).Days));//gnere nbrandom entre 0 et nb total de j calculé precedemmnt 

            s_dalAssignment.Create(new Assignment
            {
                Id = id,
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = startTime,
                EndTime = endTime,
            });

        }
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
