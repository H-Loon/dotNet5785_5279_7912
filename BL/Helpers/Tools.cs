using DalApi;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Reflection;

namespace Helpers;

internal static class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static double GetCallDistance(int callId, DO.Volunteer v)
    {
        if (v.Address is not null)
            return Haversine(s_dal.Call.Read(callId)!.Latitude, s_dal.Call.Read(callId)!.Longitude, v.Latitude!.Value, v.Longitude!.Value);
        else
            throw new ArgumentException("Volunteer address or coordinates are not valid.");
    }

    internal static BO.BoCallStatus GetCallStatus(int callId)
    {
        DO.Call call = s_dal.Call.Read(callId) ?? throw new ArgumentException("Call not found");
        DO.Assignment? assignment = s_dal.Assignment.Read(a => a.CallId == callId);

        DateTime now = ClockManager.Now;
        DateTime? maxTime = s_dal.Call.Read(callId)!.MaxTime;

        if (maxTime is not null && now > maxTime)
            return BO.BoCallStatus.OverDated;


        else if (assignment is not null)
        {
            if (assignment.EndReason is not null)
                return BO.BoCallStatus.Closed;

            else if (call.MaxTime is null)
                return BO.BoCallStatus.InTreatment;

            else if (now < maxTime - s_dal.Config.RiskRange)
                return BO.BoCallStatus.InTreatmentAndDanger;

            else
                return BO.BoCallStatus.InTreatment;
        }

        if (call.MaxTime is null)
            return BO.BoCallStatus.Open;

        else if (now < maxTime - s_dal.Config.RiskRange)
            return BO.BoCallStatus.OpenAndDanger;

        else
            return BO.BoCallStatus.Open;
    }

    internal static bool IdCheck(int id) // AI helped
    {
        {
            string idString = id.ToString();

            // Ensure the ID has 9 digits
            idString = idString.PadLeft(9, '0');

            if (idString.Length != 9 || !int.TryParse(idString, out _))
                return false;

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(idString[i].ToString());
                int product = digit * (i % 2 == 0 ? 1 : 2);
                sum += product > 9 ? product - 9 : product;
            }

            return sum % 10 == 0;
        }
    }

    internal static bool MaxDistanceCheck(double? maxDistance)
    {
        if (maxDistance is null)
            return true;
        if (maxDistance < 0 || maxDistance > 500)
            return false;
        return true;
    }

    internal static bool AddressCheck(string? address)
    {
        if (address is null)
            return true;
        try
        {
            AddressToCoordinates(address);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static bool PasswordCheck(string password)
    {
        char[] specialCharacters = { '@', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', ']', '{', '}', '|', '\\', ':', ';', '"', '\'', '<', '>', ',', '.', '/', '~', '`' };

        if (password is null)
            return true;
        if (password.Length < 6)
            return false;
        if (password.IndexOfAny(specialCharacters) >= 0)
            return false;
        if (password.Any(char.IsUpper) is false)
            return false;
        if (password.Any(char.IsLower) is false)
            return false;
        if (password.Any(char.IsDigit) is false)
            return false;

        return true;
    }

    internal static bool EmailCheck(string email) // Ai helped
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Pattern to match email addresses
        string pattern = @"^[^@\s]+@[a-z]+\.[a-z]+$";
        return Regex.IsMatch(email, pattern);
    }

    internal static bool PhoneCheck(string pn)
    {
        string pattern = @"^\+972-[0-9]{2}-[0-9]{7}$";
        return Regex.IsMatch(pn, pattern);
    }

    internal static bool NameCheck(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        string pattern = @"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$"; // Each word starts with an uppercase letter followed by one or more lowercase letters, separated by spaces
        return Regex.IsMatch(name, pattern);
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2) // Ai helped
    {
        // Radius of the Earth in kilometers
        const double R = 6371.0;

        // Convert latitude and longitude from degrees to radians
        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(lat2);
        double lon2Rad = DegreesToRadians(lon2);

        // Differences in coordinates
        double dLat = lat2Rad - lat1Rad;
        double dLon = lon2Rad - lon1Rad;

        // Haversine formula
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Distance in kilometers
        return R * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    /// <summary>
    /// Returns a string representation of the object's properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    internal static string ToStringProperty<T>(this T obj) // Ai helped
    {
        // Check if the object is null
        if (obj == null)
            return string.Empty;

        // Get the type of the object
        Type type = obj.GetType();

        // Get the name of the class
        Console.WriteLine("Class Name: " + type.Name);

        // Get the properties of the class
        PropertyInfo[] properties = type.GetProperties();
        string result = string.Empty;

        foreach (var property in properties)
        {
            object? value = property.GetValue(obj);
            result += $"{property.Name}: {value}\n";
        }

        return result;
    }

    public static (double Latitude, double Longitude) AddressToCoordinates(string address) // Ai helped
    {
        // Geocoding API key
        string apiKey = "6754830d05d34753981159dre426e6e";
        string format = "xml";

        // Encode the address to make it URL-safe
        string encodedAddress = Uri.EscapeDataString(address);

        // Geocoding API URL for XML format
        string url = $"https://geocode.maps.co/search?q={encodedAddress}&api_key={apiKey}&format={format}";

        using HttpClient client = new HttpClient();

        try
        {
            // Synchronously send the HTTP GET request and get the response
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            // Read the response content synchronously
            string xmlResponse = response.Content.ReadAsStringAsync().Result;

            // Parse the XML response
            XDocument doc = XDocument.Parse(xmlResponse);

            // Extract latitude and longitude from the XML
            var placeElement = doc.Root?.Element("place");
            if (placeElement == null)
                throw new Exception("Could not find coordinates for the given address.");

            double latitude = double.Parse(placeElement.Attribute("lat")!.Value);
            double longitude = double.Parse(placeElement.Attribute("lon")!.Value);

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get coordinates from the address.", ex);
        }
    }
}
