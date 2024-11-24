namespace DalTest;
using DalApi;
using DO;
using System.Net;
using System.Numerics;

public static class Initialization
{
    // The DAL instance
    private static IDal? s_dal;

    // Random number generator
    private static readonly Random s_rand = new();

    // Sample data arrays
    private static readonly string[] s_names =
    [
        "David Cohen", "Yossi Levi", "Miriam Katz", "Rivka Goldberg", "Moshe Ben-David",
        "Sara Peretz", "Yaakov Shapiro", "Rachel Mizrahi", "Shlomo Rosen", "Esther Friedman",
        "Noa Bar", "Elior Shalom", "Tamar Levi", "Avi Cohen", "Lior Ben-Ami", "Hila Shalev"
    ];

    private static readonly string[] s_emails =
    [
        "david.cohen@example.com", "yossi.levi@example.com", "miriam.katz@example.com",
        "rivka.goldberg@example.com", "moshe.ben-david@example.com", "sara.peretz@example.com",
        "yaakov.shapiro@example.com", "rachel.mizrahi@example.com", "shlomo.rosen@example.com",
        "esther.friedman@example.com", "noa.bar@example.com", "elior.shalom@example.com",
        "tamar.levi@example.com", "avi.cohen@example.com", "lior.ben-ami@example.com",
        "hila.shalev@example.com"
    ];

    private static readonly List<string> s_phoneNumbers =
    [
        "+972-50-1234567", "+972-52-2345678", "+972-54-3456789", "+972-55-4567890",
        "+972-56-5678901", "+972-57-6789012", "+972-58-7890123", "+972-59-8901234",
        "+972-50-9012345", "+972-52-0123456", "+972-50-6543210", "+972-52-7654321",
        "+972-54-8765432", "+972-55-9876543", "+972-56-0987654", "+972-57-1098765"
    ];

    private static readonly List<string> s_addresses =
    [
        "1 Rothschild Blvd, Tel Aviv", "2 Herzl St, Haifa", "3 Jabotinsky St, Ramat Gan",
        "4 Ben Yehuda St, Jerusalem", "5 Dizengoff St, Tel Aviv", "6 Weizmann St, Rehovot",
        "7 Begin Blvd, Beersheba", "8 Allenby St, Tel Aviv", "9 King George St, Jerusalem",
        "10 Bialik St, Ramat Gan", "11 Arlozorov St, Tel Aviv", "12 Ibn Gabirol St, Tel Aviv",
        "13 Kaplan St, Tel Aviv", "14 Namir Rd, Tel Aviv", "15 Hahashmonaim St, Tel Aviv",
        "16 Yehuda Halevi St, Tel Aviv", "17 Frishman St, Tel Aviv", "18 Dizengoff Square, Tel Aviv",
        "19 Sheinkin St, Tel Aviv", "20 Florentin St, Tel Aviv", "21 Rothschild Blvd, Tel Aviv",
        "22 Herzl St, Haifa", "23 Jabotinsky St, Ramat Gan", "24 Ben Yehuda St, Jerusalem",
        "25 Dizengoff St, Tel Aviv", "26 Weizmann St, Rehovot", "27 Begin Blvd, Beersheba",
        "28 Allenby St, Tel Aviv", "29 King George St, Jerusalem", "30 Bialik St, Ramat Gan",
        "31 Arlozorov St, Tel Aviv", "32 Ibn Gabirol St, Tel Aviv", "33 Kaplan St, Tel Aviv",
        "34 Namir Rd, Tel Aviv", "35 Hahashmonaim St, Tel Aviv", "36 Yehuda Halevi St, Tel Aviv",
        "37 Frishman St, Tel Aviv", "38 Dizengoff Square, Tel Aviv", "39 Sheinkin St, Tel Aviv",
        "40 Florentin St, Tel Aviv"
    ];

    private static readonly List<double> s_latitudes =
    [
        32.065, 32.818, 32.083, 31.776, 32.075, 31.894, 31.252, 32.067, 31.780, 32.082,
        32.070, 32.073, 32.074, 32.075, 32.076, 32.077, 32.078, 32.079, 32.080, 32.081,
        32.065, 32.818, 32.083, 31.776, 32.075, 31.894, 31.252, 32.067, 31.780, 32.082,
        32.070, 32.073, 32.074, 32.075, 32.076, 32.077, 32.078, 32.079, 32.080, 32.081
    ];

    private static readonly List<double> s_longitudes =
    [
        34.774, 34.988, 34.814, 35.213, 34.774, 34.811, 34.791, 34.770, 35.220, 34.814,
        34.780, 34.781, 34.782, 34.783, 34.784, 34.785, 34.786, 34.787, 34.788, 34.789,
        34.774, 34.988, 34.814, 35.213, 34.774, 34.811, 34.791, 34.770, 35.220, 34.814,
        34.780, 34.781, 34.782, 34.783, 34.784, 34.785, 34.786, 34.787, 34.788, 34.789
    ];

    private static readonly string[] s_callDescriptions =
    [
        "My HomeBot is stuck in a loop!", "The teleporter is blocked at Machon Lev!",
        "My dishwasher is in depression!", "My time travel machine refuses to work!",
        "I have an unidentified issue!", "HomeBot is trying to take over the house!",
        "Teleporter sent me to the wrong place!", "Dishwasher is throwing a tantrum!",
        "Time travel machine sent me to the future!", "Other strange issue!"
    ];

    /// <summary>
    /// Creates sample volunteers and adds them to the DAL.
    /// </summary>
    private static void CreateVolunteer()
    {
        List<string> copyAddresses = new(s_addresses);
        List<double> copyLatitudes = new(s_latitudes);
        List<double> copyLongitudes = new(s_longitudes);

        s_dal!.Volunteer.Create(new Volunteer
        {
            Id = s_rand.Next(200000000, 400000000),
            Name = s_names[0],
            Phone = s_phoneNumbers[0],
            Email = s_emails[0],
            Address = s_addresses[0],
            Latitude = s_latitudes[0],
            Longitude = s_longitudes[0],
            IsActive = s_rand.Next(0, 2) is 1,
            MaxDistance = s_rand.Next(1, 100),
            Role = RoleType.Admin
        });

        copyAddresses.RemoveAt(0);
        copyLatitudes.RemoveAt(0);
        copyLongitudes.RemoveAt(0);

        for (int i = 1; i < s_names.Length; i++)
        {
            int id;
            string phone, address;
            double latitude, longitude;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dal!.Volunteer.Read(id) != null);

            phone = s_phoneNumbers[s_rand.Next(0, s_phoneNumbers.Count)];
            s_phoneNumbers.Remove(phone);

            int r = s_rand.Next(0, s_addresses.Count);
            address = copyAddresses[r];
            latitude = copyLatitudes[r];
            longitude = copyLongitudes[r];
            copyAddresses.RemoveAt(r);
            copyLatitudes.RemoveAt(r);
            copyLongitudes.RemoveAt(r);

            s_dal!.Volunteer.Create(new Volunteer
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
                Role = RoleType.Volunteer
            });
        }
    }

    /// <summary>
    /// Generates a random date within a specified range.
    /// </summary>
    /// <returns>A random DateTime value.</returns>
    private static DateTime RandomDate()
    {
        DateTime startTime = new(s_dal!.Config.Clock.Year - s_rand.Next(-5, 0), 12, 24);
        int range = (s_dal!.Config.Clock - startTime).Days;
        return startTime.AddDays(s_rand.Next(range));
    }

    /// <summary>
    /// Creates sample assignments and adds them to the DAL.
    /// </summary>
    private static void CreateAssignment()
    {
        List<Call> lc = s_dal!.Call.ReadAll(c => c.MaxTime>s_dal!.Config.Clock).ToList();
        List<Volunteer> lv = s_dal!.Volunteer.ReadAll().ToList();

        int callId, volunteerId;

        for (int i = 0; i < 15; i++)
        {
            DateTime startDate = RandomDate();

            int maxDays = (s_dal!.Config.Clock - startDate).Days;

            int index = s_rand.Next(0, lc.Count);

            callId = lc[index].Id;
            lc.RemoveAt(index);

            index = s_rand.Next(0, lv.Count);

            volunteerId = lv[i].Id;

            s_dal!.Assignment.Create(new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartDate = startDate,
                EndDate = null,
                EndReason = null
            });
        }
        for (int i = 0; i < 35; i++)
        {
            DateTime startDate = RandomDate();

            int maxDays = (s_dal!.Config.Clock - startDate).Days;

            int index = s_rand.Next(0, lc.Count);

            callId = lc[index].Id;
            lc.RemoveAt(index);

            index = s_rand.Next(0, lv.Count);

            volunteerId = lv[index].Id;

            DateTime? endDate =startDate.AddDays(s_rand.Next(0, maxDays));
            s_dal!.Assignment.Create(new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartDate = startDate,
                EndDate = endDate,
                EndReason = (AssignmentEndReason)s_rand.Next(0, 4)
            });
        }
    }

    /// <summary>
    /// Creates sample calls and adds them to the DAL.
    /// </summary>
    private static void CreateCall()
    {
        for (int i = 0; i < 5; i++)
        {
            DateTime startDate = RandomDate();
            int index = s_rand.Next(0, s_addresses.Count);
            s_dal!.Call.Create(new Call
            {
                Type = (CallType)s_rand.Next(0, 5),
                Address = s_addresses[index],
                Latitude = s_latitudes[index],
                Longitude = s_longitudes[index],
                StartTime = startDate,
                Description = s_callDescriptions[s_rand.Next(0, s_callDescriptions.Length)], // Random description
                MaxTime = s_dal!.Config.Clock.AddDays(s_rand.Next(-((s_dal!.Config.Clock - startDate).Days),0)) // Random date between start date and today
            });
        }
        for (int i = 0; i < 60; i++)
        {
            int index = s_rand.Next(0, s_addresses.Count);
            s_dal!.Call.Create(new Call
            {
                Type = (CallType)s_rand.Next(0, 5),
                Address = s_addresses[index],
                Latitude = s_latitudes[index],
                Longitude = s_longitudes[index],
                StartTime = RandomDate(),
                Description = s_callDescriptions[s_rand.Next(0, s_callDescriptions.Length)],
                MaxTime = s_dal!.Config.Clock.AddDays(s_rand.Next(364))
            });
        }
    }

    /// <summary>
    /// Initializes the DAL with sample data.
    /// </summary>
    /// <param name="dal">The DAL instance.</param>
    public static void Do(IDal dal)
    {
        s_dal = dal ?? throw new NullReferenceException("DAL object cannot be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();

        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteer();

        Console.WriteLine("Initializing Calls list ...");
        CreateCall();

        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignment();
    }
}
