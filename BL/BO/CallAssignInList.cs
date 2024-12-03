using Helpers;

namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? VolunteerName { get; init; }
    public DateTime AssignTime { get; init; }
    public DateTime? EndedTime { get; init; }
    public BoAssignmentEndReason? EndType { get; init; }
    public override string ToString() => this.ToStringProperty();
}
