using DalApi;
using System.Xml.Linq;
using System.Reflection;
using System.Net.Mail;
using BO;
using System.Collections.Generic;

namespace Helpers;

internal static class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static double GetCallDistance(int callId, DO.Volunteer v)
    {
        if (v.Address is not null)
        {
            var call = s_dal.Call.Read(callId);
            return Haversine(call!.Latitude, call!.Longitude, v.Latitude!.Value, v.Longitude!.Value);
        }
        else
            throw new BO.BlAddressNotValidException("Volunteer address is not valid.");
    }

    internal static bool AddressCheck(string? address)
    {
        if (address is null)
            return true;
        try
        {
            AddressToCoordinates(address); // if the address is not valid, an exception will be thrown otherwise it will return true
            return true;
        }
        catch
        {
            return false; // if an exception is thrown, the address is not valid
        }
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
            if (value is double doubleValue)
            {
                result += $"{property.Name}: {doubleValue.ToString("G", System.Globalization.CultureInfo.InvariantCulture)}\n"; // Format double to general format e.g. 1.1 instead of 11
            }
            else if (value is List<BO.CallAssignInList>)
            {
                result += $"{property.Name}:";
                foreach (var item in (List<BO.CallAssignInList>)value)
                {
                    result += $"\t{item}\n";
                }
            }
            else
            {
                result += $"{property.Name}: {value}\n";
            }

        }
        return result;
    }

    internal static (double Latitude, double Longitude) AddressToCoordinates(string address) // Ai helped
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
                throw new BO.BlCoordinatesNotFoundException("Could not find coordinates for the given address.");

            double latitude = double.Parse(placeElement.Attribute("lat")!.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            double longitude = double.Parse(placeElement.Attribute("lon")!.Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get coordinates from the address.", ex);
        }
    }

    internal static void SendEmail(string sender, string receiver, string subject, string msg) // Ai helped
    {
        // Local SMTP server settings (Papercut)
        string smtpServer = "127.0.0.1"; // Localhost
        int smtpPort = 25;               // Default Papercut port

        try
        {
            // Create a MailMessage object
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(sender); // Fake sender address for testing
            mail.To.Add(receiver);                       // Recipient email address
            mail.Subject = subject;                      // Email subject
            mail.Body = msg;                             // Email content

            // Configure the SmtpClient
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,  // Send via network
                UseDefaultCredentials = false,                // No authentication needed for Papercut
                EnableSsl = false                             // No encryption required
            };

            // Send the email
            smtpClient.Send(mail);

        }
        catch (Exception ex)
        {
            throw new BlEmailNotSendException("Error sending email: " + ex.Message);
        }
    }

    
}
