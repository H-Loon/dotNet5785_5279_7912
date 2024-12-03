namespace BO;

public enum BoRoleType
{
    Volunteer,
    Admin
}

public enum BoDistanceType
{
    Area,
    Walking,
    Car
}

public enum BoCallType
{
    HomeBotIssue,
    TeleporterBlockedOnMachonLev,
    MyDishwasherIsInDepression,
    MyTimeTravelMachineIsLazy,
    Other
}

public enum BoCallStatus
{
    Open,
    OpenAndDanger,
    InTreatment,
    InTreatmentAndDanger,
    Closed,
    OverDated
}

public enum BoAssignmentEndReason
{
    Completed,
    CanceledByVolunteer,
    CanceledByAdmin,
    OverDated
}

public enum TimeType
{
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Year
}

public enum VolunteerInListField
{
    Id,
    Name,
    Active,
    CompletedCalls,
    CanceledCalls,
    CallInTreatment,
    CurrentCallType
}
public enum CallInListField
{
    AssignmentId,
    CallId,
    CallType,
    StartTime,
    TimeLeft,
    LastVolunteerName,
    TimeOpen,
    CallStatus,
    AssignCount
}
public enum OpenCallInListField
{
    Id,
    CallType,
    Description,
    Address,
    StartTime,
    MaxTime,
    CallDistance,
}
public enum ClosedCallInListField
{
    Id,
    CallType,
    Address,
    StartTime,
    AssignTime,
    EndedTime,
    EndType
}