namespace DO;

public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime StartTime,
    DateTime? EndTime = null,
    AssignmentEndReason? EndReason = null
)
{
    public Assignment() : this(0, 0, 0, DateTime.Now) { }
}
