namespace DO;
/// <summary>
/// Link between a call and a volunteer
/// </summary>
/// <param name="Id">The Id of the Assignement</param>
/// <param name="CallId">The Call Id</param>
/// <param name="VolunteerId">The Volunteer Id </param>
/// <param name="StartTime">The time when the volunteer accepted the call</param>
/// <param name="EndTime">The time when the call has been closed</param>
/// <param name="EndReason">The reason why the call has been closed</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime StartDate,
    DateTime? EndDate = null,
    AssignmentEndReason? EndReason = null
)
{
    public Assignment() : this(0, 0, 0, DateTime.Now) { } // for ch3
}
