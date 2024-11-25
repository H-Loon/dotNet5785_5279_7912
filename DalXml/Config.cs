namespace Dal;

/// <summary>
/// Static class to manage configuration settings for the application.
/// </summary>
internal static class Config
{
    // Constants for XML file names
    internal const string Data_Config = "data-config.xml";
    internal const string Volunteers_Xml = "volunteers.xml";
    internal const string Calls_Xml = "calls.xml";
    internal const string Assignments_Xml = "assignments.xml";

    /// <summary>
    /// Gets the next call ID and increments it in the configuration.
    /// </summary>
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(Data_Config, "NextCallId");
        private set => XMLTools.SetConfigIntVal(Data_Config, "NextCallId", value);
    }

    /// <summary>
    /// Gets the next assignment ID and increments it in the configuration.
    /// </summary>
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(Data_Config, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(Data_Config, "NextAssignmentId", value);
    }

    /// <summary>
    /// Gets or sets the current clock value in the configuration.
    /// </summary>
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(Data_Config, "Clock");
        set => XMLTools.SetConfigDateVal(Data_Config, "Clock", value);
    }

    /// <summary>
    /// Gets or sets the risk range time span in the configuration.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(Data_Config, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(Data_Config, "RiskRange", value);
    }

    /// <summary>
    /// Resets the configuration values to their default settings.
    /// </summary>
    internal static void Reset()
    {
        NextCallId = 1000;
        NextAssignmentId = 2000;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromDays(1);
    }
}
