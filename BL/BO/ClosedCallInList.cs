using Helpers;

namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }
    public BoCallType CallType { get; set; }
    public required string Address { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime AssignTime { get; set; }
    public DateTime EndedTime { get; set; }
    public BoAssignmentEndReason EndType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
