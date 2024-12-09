using Helpers;

namespace BO;

public class CallInProgress
{
    public int AssignmentId { get; init; }
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public BoCallType CallType { get; set; }
    public string? Description { get; set; }
    public required string Address { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? MaxTime { get; set; }
    public DateTime AssignTime { get; set; }
    public double CallDistance { get; set; }
    public BoCallStatus Status { get; set; }
    public override string ToString() => this.ToStringProperty();
}
