namespace Dal;
internal static class Config
{
    // Id number for the next new read 
    internal const int startCallId = 1000; 
    private static int nextCallId = startCallId;
    internal static int NextCallId => nextCallId++;

    // Id number Assignment
    internal const int startAssignmementId = 2000;
    private static int nextAssignmementId = startAssignmementId;
    internal static int NextAssignmementId => nextAssignmementId++;

    // System clock editable 
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // Period risk
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromDays(1);// previent du danger 1j avant 

    // Reset all entity
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmementId = startAssignmementId;

        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromDays(1);
    }
}

