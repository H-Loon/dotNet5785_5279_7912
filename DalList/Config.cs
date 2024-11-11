namespace Dal;
internal static class Config
{
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId => nextCallId++;
}

