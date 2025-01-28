using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// Static class to manage configuration settings for the application.
/// </summary>
internal static class Config
{
    // Starting ID number for the next new call
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;

    /// <summary>
    /// Gets the next call ID and increments it.
    /// </summary>
    internal static int NextCallId => nextCallId++;

    // Starting ID number for the next new assignment
    internal const int startAssignmementId = 2000;
    private static int nextAssignmementId = startAssignmementId;

    /// <summary>
    /// Gets the next assignment ID and increments it.
    /// </summary>
    internal static int NextAssignmentId => nextAssignmementId++;

    /// <summary>
    /// Gets or sets the current clock value.
    /// </summary>
    
    internal static DateTime Clock { 
        [MethodImpl(MethodImplOptions.Synchronized)] 
        get; 
        [MethodImpl(MethodImplOptions.Synchronized)] 
        set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the risk range time span.
    /// </summary>
    
    internal static TimeSpan RiskRange {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set; } = TimeSpan.FromDays(1); // Warns of danger 1 day in advance

    /// <summary>
    /// Resets all configuration values to their default settings.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmementId = startAssignmementId;

        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromDays(1);
    }
}

