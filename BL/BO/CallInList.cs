using Helpers;

namespace BO;

public class CallInList
{
    public int? AssignmentId { get; init; }
    public int CallId { get; init; }
    public BoCallType CallType { get; init; }
    public DateTime StartTime { get; init; }
    public TimeSpan? TimeLeft { get; init; }
    public string? LastVolunteerName { get; init; }
    public TimeSpan? TimeOpen { get; init; }
    public BoCallStatus CallStatus { get; init; }
    public int AssignCount { get; init; }
    public override string ToString() => this.ToStringProperty();

}
