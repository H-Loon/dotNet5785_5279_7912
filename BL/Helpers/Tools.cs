using System.Reflection;

namespace Helpers;

internal static class Tools
{
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
            object value = property.GetValue(obj)!;
            result += $"{property.Name}: {value}\n";
        }

        return result;
    }
}
