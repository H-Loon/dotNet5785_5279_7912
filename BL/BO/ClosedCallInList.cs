using Helpers;

namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }
    public BoCallType CallType { get; init; }
    public required string Address { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime AssignTime { get; init; }
    public DateTime EndedTime { get; init; }
    public BoAssignmentEndReason EndType { get; init; }
    public override string ToString() => this.ToStringProperty();
}
