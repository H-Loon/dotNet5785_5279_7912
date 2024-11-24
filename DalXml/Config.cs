namespace Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal static class Config
{
    internal const string Data_Config= "data_config";
    internal const string Volunteers_Xml = "volunteers.xml";
    internal const string Calls_Xml = "calls.xml";
    internal const string Assignment_Xml = "assignments.xml";

    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(Data_Config, "NextCallId");
        private set => XMLTools.SetConfigIntVal(Data_Config, "NextCallId", value);
    }

    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(Data_Config, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(Data_Config, "NextAssignmentId", value);
    }

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(Data_Config, "Clock");
        set => XMLTools.SetConfigDateVal(Data_Config, "Clock", value);
    }

    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(Data_Config, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(Data_Config, "RiskRange", value);
    }

    internal static void Reset()
    {
        NextCallId = 1000;
        NextAssignmentId = 2000;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromDays(1);
    }
}
