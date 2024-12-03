using Helpers;

namespace BO;

public class CallInList
{
    public int AssignmentId { get; set; }
    public int CallId { get; init; }
    public BoCallType CallType { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan? TimeLeft { get; set; }
    public string? LastVolunteerName { get; set; }
    public TimeSpan? TimeOpen { get; set; }
    public BoCallStatus CallStatus { get; set; }
    public int AssignCount { get; set; }
    public override string ToString() => this.ToStringProperty();

}
