using DalApi;
using DO;

namespace DalTest;
public static class Initialization
{
    // The DAL instance
    private static IDal? s_dal;

    // Random number generator
    private static readonly Random s_rand = new();

    // Sample data arrays
    private static readonly string[] s_names =
    [
        "David Cohen","David Locohen", "Yossi Levi", "Miriam Katz", "Rivka Goldberg", "Moshe Ben David",
        "Sara Peretz", "Yaakov Shapiro", "Rachel Mizrahi", "Shlomo Rosen", "Esther Friedman",
        "Noa Bar", "Elior Shalom", "Tamar Levi", "Avi Cohen", "Lior Ben Ami", "Hila Shalev"
    ];

    private static readonly string[] s_emails =
    [
        "david.cohen@example.com", "david.locohen@example.com", "yossi.levi@example.com", "miriam.katz@example.com",
        "rivka.goldberg@example.com", "moshe.ben-david@example.com", "sara.peretz@example.com",
        "yaakov.shapiro@example.com", "rachel.mizrahi@example.com", "shlomo.rosen@example.com",
        "esther.friedman@example.com", "noa.bar@example.com", "elior.shalom@example.com",
        "tamar.levi@example.com", "avi.cohen@example.com", "lior.ben-ami@example.com",
        "hila.shalev@example.com"
    ];

    private static readonly string[] s_passwords = new string[]
    {
        "Password1!","Password1!", "Secure2Pass@", "Strong3Pwd#", "Valid4Pass$", "Check5Pass%",
        "Safe6Passw^", "Good7Passw&", "Right8Pass*", "Valid9Pass(", "Strong0Pwd)",
        "Check1Pass-", "Safe2Passw_", "Good3Passw=", "Right4Pass+", "Valid5Pass[",
        "Strong6Pwd]"  
    };

    private static readonly List<string> s_phoneNumbers =
    [
        "0501234567","0501234568", "0522345678", "0543456789", "0554567890",
        "0565678901", "0576789012", "0587890123", "0598901234",
        "0509012345", "0520123456", "0506543210", "0527654321",
        "0548765432", "0559876543", "0560987654", "0571098765"
    ];

    private static readonly List<string> s_addresses =
    [
        "1 Rothschild Blvd, Tel Aviv","2 Rothschild Blvd, Tel Aviv", "2 Herzl St, Haifa", "3 Jabotinsky St, Ramat Gan",
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
        "40 Florentin St, Tel Aviv", "41 Allenby St, Haifa", "42 Ben Gurion Blvd, Haifa",
        "43 HaNassi Blvd, Haifa", "44 Haifa Port, Haifa", "45 Carmel Beach, Haifa",
        "46 Technion City, Haifa", "47 University of Haifa, Haifa", "48 Haifa Bay, Haifa",
        "49 Haifa Mall, Haifa", "50 Haifa Zoo, Haifa", "51 Haifa Museum, Haifa",
        "52 Haifa Theater, Haifa", "53 Haifa Cinematheque, Haifa", "54 Haifa Auditorium, Haifa",
        "55 Haifa Stadium, Haifa", "56 Haifa Airport, Haifa", "57 Haifa Train Station, Haifa",
        "58 Haifa Bus Station, Haifa", "59 Haifa Central Station, Haifa", "60 Haifa South Station, Haifa"
    ];

    private static readonly List<double> s_latitudes =
    [
        32.065, 32.818, 32.065, 32.083, 31.776, 32.075, 31.894, 31.252, 32.067, 31.780, 32.082,
        32.070, 32.073, 32.074, 32.075, 32.076, 32.077, 32.078, 32.079, 32.080, 32.081,
        32.065, 32.818, 32.083, 31.776, 32.075, 31.894, 31.252, 32.067, 31.780, 32.082,
        32.070, 32.073, 32.074, 32.075, 32.076, 32.077, 32.078, 32.079, 32.080, 32.081,
        32.818, 32.819, 32.820, 32.821, 32.822, 32.823, 32.824, 32.825, 32.826, 32.827,
        32.828, 32.829, 32.830, 32.831, 32.832, 32.833, 32.834, 32.835, 32.836, 32.837
    ];

    private static readonly List<double> s_longitudes =
    [
        34.774, 34.774, 34.988, 34.814, 35.213, 34.774, 34.811, 34.791, 34.770, 35.220, 34.814,
        34.780, 34.781, 34.782, 34.783, 34.784, 34.785, 34.786, 34.787, 34.788, 34.789,
        34.774, 34.988, 34.814, 35.213, 34.774, 34.811, 34.791, 34.770, 35.220, 34.814,
        34.780, 34.781, 34.782, 34.783, 34.784, 34.785, 34.786, 34.787, 34.788, 34.789,
        34.988, 34.989, 34.990, 34.991, 34.992, 34.993, 34.994, 34.995, 34.996, 34.997,
        34.998, 34.999, 35.000, 35.001, 35.002, 35.003, 35.004, 35.005, 35.006, 35.007
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
    private static void CreateVolunteers()
    {
        List<string> copyAddresses = new(s_addresses);
        List<double> copyLatitudes = new(s_latitudes);
        List<double> copyLongitudes = new(s_longitudes);
        List<string> copyPhones = new(s_phoneNumbers);

        s_dal!.Volunteer.Create(new Volunteer
        {
            Id = 238336280,
            Name = s_names[0],
            Phone = s_phoneNumbers[0],
            Email = s_emails[0],
            Password = s_passwords[0],
            Address = s_addresses[0],
            Latitude = s_latitudes[0],
            Longitude = s_longitudes[0],
            IsActive = true,
            MaxDistance = s_rand.Next(50, 200),
            Role = RoleType.Admin
        });

        copyAddresses.RemoveAt(0);
        copyLatitudes.RemoveAt(0);
        copyLongitudes.RemoveAt(0);

        s_dal!.Volunteer.Create(new Volunteer
        {
            Id = 256810680,
            Name = s_names[1],
            Phone = s_phoneNumbers[1],
            Email = s_emails[1],
            Password = s_passwords[1],
            Address = s_addresses[1],
            Latitude = s_latitudes[1],
            Longitude = s_longitudes[1],
            IsActive = true,
            MaxDistance = s_rand.Next(50, 200),
            Role = RoleType.Admin
        });

        copyAddresses.RemoveAt(1);
        copyLatitudes.RemoveAt(1);
        copyLongitudes.RemoveAt(1);

        for (int i = 2; i < s_names.Length; i++)
        {
            int id;
            string phone, address;
            double latitude, longitude;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dal!.Volunteer.Read(id) != null);

            phone = copyPhones[s_rand.Next(0, copyPhones.Count)];
            copyPhones.Remove(phone);

            int r = s_rand.Next(0, copyAddresses.Count);
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
                Password = s_passwords[i],
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
    private static DateTime RandomDate(DateTime dateTime = default(DateTime), bool flag = true)
    {
        if (flag)
        {
            DateTime startTime = new(s_dal!.Config.Clock.Year - s_rand.Next(5), 1, 1);
            int range1 = (s_dal!.Config.Clock - startTime).Days;
            return startTime.AddDays(s_rand.Next(range1));
        }
        int range2 = (s_dal!.Config.Clock - dateTime).Days;
        return dateTime.AddDays(s_rand.Next(range2));

    }

    /// <summary>
    /// Creates sample assignments and adds them to the DAL.
    /// </summary>
    private static void CreateAssignments()
    {
        List<Volunteer> lv = s_dal!.Volunteer.ReadAll().ToList();
        List<Call> lc = s_dal!.Call.ReadAll(c => c.MaxTime > s_dal!.Config.Clock).ToList();

        int callId, volunteerId;

        for (int i = 0; i < 15; i++)
        {
            int index = s_rand.Next(0, lc.Count);

            callId = lc[index].Id;
            DateTime startDate = RandomDate(lc[index].StartTime, false);
            lc.RemoveAt(index);

            int daysBetween = (startDate - s_dal!.Config.Clock).Days;

            index = s_rand.Next(0, lv.Count);

            volunteerId = lv[i].Id;

            s_dal!.Assignment.Create(new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = startDate,
                EndTime = null,
                EndReason = null
            });
        }
        for (int i = 0; i < 35; i++)
        {
            int index = s_rand.Next(0, lc.Count);

            callId = lc[index].Id;
            DateTime startDate = RandomDate(lc[index].StartTime,false);
            lc.RemoveAt(index);


            int daysBetween = (startDate - s_dal!.Config.Clock).Days;

            index = s_rand.Next(0, lv.Count);

            volunteerId = lv[index].Id;

            DateTime? endDate = startDate.AddDays(s_rand.Next(daysBetween,0));
            s_dal!.Assignment.Create(new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = startDate,
                EndTime = endDate,
                EndReason = (AssignmentEndReason)s_rand.Next(0, 4)
            });
        }
    }

    /// <summary>
    /// Creates sample calls and adds them to the DAL.
    /// </summary>
    private static void CreateCalls()
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
                MaxTime = s_dal!.Config.Clock.AddDays(s_rand.Next((startDate - s_dal!.Config.Clock).Days, 0)) // Random date between start date and today
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
    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        //s_dal = dal ?? throw new NullReferenceException("DAL object cannot be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();

        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers();

        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();

        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignments();
    }
}
