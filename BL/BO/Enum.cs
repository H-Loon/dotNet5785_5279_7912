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