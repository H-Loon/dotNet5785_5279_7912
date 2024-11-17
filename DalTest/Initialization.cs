namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();

    // the following arrays are made by AI (copilot)
    private static readonly string[] s_names =
    [
                "David Cohen", "Yossi Levi", "Miriam Katz", "Rivka Goldberg", "Moshe Ben-David",
                "Sara Peretz", "Yaakov Shapiro", "Rachel Mizrahi", "Shlomo Rosen", "Esther Friedman"
            ];

    private static readonly string[] s_emails =
    [
                "david.cohen@example.com", "yossi.levi@example.com", "miriam.katz@example.com",
                "rivka.goldberg@example.com", "moshe.ben-david@example.com", "sara.peretz@example.com",
                "yaakov.shapiro@example.com", "rachel.mizrahi@example.com", "shlomo.rosen@example.com",
                "esther.friedman@example.com"
            ];

    private static readonly List<string> s_phoneNumbers =
    [
                "+972-50-1234567", "+972-52-2345678", "+972-54-3456789", "+972-55-4567890",
                "+972-56-5678901", "+972-57-6789012", "+972-58-7890123", "+972-59-8901234",
                "+972-50-9012345", "+972-52-0123456"
            ];

    private static readonly List<string> s_addresses =
    [
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
            ];

    private static readonly List<double> s_latitudes =
    [
                32.065, 32.818, 32.083, 31.776, 32.075,
                31.894, 31.252, 32.067, 31.780, 32.082
            ];

    private static readonly List<double> s_longitudes =
    [
                34.774, 34.988, 34.814, 35.213, 34.774,
                34.811, 34.791, 34.770, 35.220, 34.814
            ];

    private static readonly string[] s_callDescriptions =
    [
                "My HomeBot is stuck in a loop!",
                "The teleporter is blocked at Machon Lev!",
                "My dishwasher is in depression!",
                "My time travel machine refuses to work!",
                "I have an unidentified issue!",
                "HomeBot is trying to take over the house!",
                "Teleporter sent me to the wrong place!",
                "Dishwasher is throwing a tantrum!",
                "Time travel machine sent me to the future!",
                "Other strange issue!"
            ];

    private static void _createVolunteer()
    {
        List<string> copyAddresses = new(s_addresses);
        List<double> copyLatitudes = new(s_latitudes);
        List<double> copyLongitudes = new(s_longitudes);
        for (int i = 0; i < s_names.Length; i++)
        {
            int id;
            string phone, address;
            double latitude, longitude;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalVolunteer!.Read(id) != null);


            phone = s_phoneNumbers[s_rand.Next(0, s_phoneNumbers.Count)];
            s_phoneNumbers.Remove(phone);

            int r = s_rand.Next(0, s_addresses.Count);
            address = copyAddresses[r];
            latitude = copyLatitudes[r];
            longitude = copyLongitudes[r];
            copyAddresses.RemoveAt(r);
            copyLatitudes.RemoveAt(r);
            copyLongitudes.RemoveAt(r);

            s_dalVolunteer.Create(new Volunteer
            {
                Id = id,
                Name = s_names[i],
                Phone = phone,
                Email = s_emails[i],
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                IsActive = s_rand.Next(0, 2) is 1,
                MaxDistance = s_rand.Next(1, 100),
                Role = (RoleType)s_rand.Next(0, 2)
            });
        }
    }

    private static DateTime _randomDate()
    {
        DateTime startTime = new DateTime(s_dalConfig!.Clock.Year - s_rand.Next(-5, 0), 12, 24);
        int range = (s_dalConfig!.Clock - startTime).Days;
        return startTime.AddDays(s_rand.Next(range));
    }

    private static void _createAssignment()
    {
        List<Call> lc = s_dalCall!.ReadAll();

        List<Volunteer> lv = s_dalVolunteer!.ReadAll();

        int callId, volunteerId;

        for (int i = 0; i < 5; i++)
        {
            int index = s_rand.Next(0, lc.Count);

            callId = lc[index].Id;
            lc.RemoveAt(index);

            index = s_rand.Next(0, lv.Count);

            volunteerId = lv[index].Id;
            lv.RemoveAt(index);

            /*
            List<Assignment> la = s_dalAssignment!.ReadAll();

            do
            {
                index = s_rand.Next(0, lv.Count);
                volunteerId = lv[index].Id;

            } while (la.Any(a => a.VolunteerId == volunteerId && a.EndReason == null)); maybe we need to check if the volunteer is already assigned */

            s_dalAssignment!.Create(new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartDate = _randomDate()
            });
        }
    }

    private static void _createCall()
    {
        for (int i = 0; i < 5; i++)
        {
            int index = s_rand.Next(0, s_addresses.Count);
            s_dalCall!.Create(new Call
            {
                Type = (CallType)s_rand.Next(0, 5),
                Address = s_addresses[index],
                Latitude = s_latitudes[index],
                Longitude = s_longitudes[index],
                StartTime = _randomDate(),
                Description = s_callDescriptions[s_rand.Next(0, s_callDescriptions.Length)],
                MaxTime = s_dalConfig!.Clock.AddDays(s_rand.Next(364))
            });
        }
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

        Console.WriteLine("Initializing Volunteers list ...");
        _createVolunteer();

        Console.WriteLine("Initializing Calls list ...");
        _createCall();

        Console.WriteLine("Initializing Assignments list ...");
        _createAssignment();
    }
}
