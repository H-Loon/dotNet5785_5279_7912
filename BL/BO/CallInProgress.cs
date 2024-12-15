using Helpers;

namespace BO;

public class CallInProgress
{
    public int AssignmentId { get; init; }
    public int CallId { get; init; }
    public int VolunteerId { get; init; }
    public BoCallType CallType { get; init; }
    public string? Description { get; init; }
    public required string Address { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? MaxTime { get; init; }
    public DateTime AssignTime { get; init; }
    public double CallDistance { get; init; }
    public BoCallStatus Status { get; init; }
    public override string ToString() => this.ToStringProperty();
}
