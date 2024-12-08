using DalApi;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static bool ValidateCall(BO.Call call)
{
    if (string.IsNullOrWhiteSpace(call.Address))
        throw new ArgumentException("Address cannot be null or empty.");

    if (call.Id <= 0)
        throw new ArgumentException("Id must be a positive integer.");

    if (call.Latitude < -90 || call.Latitude > 90)
        throw new ArgumentException("Latitude must be between -90 and 90.");

    if (call.Longitude < -180 || call.Longitude > 180)
        throw new ArgumentException("Longitude must be between -180 and 180.");

    if (call.StartTime == default)
        throw new ArgumentException("StartTime must be a valid date.");

    if (call.MaxTime.HasValue && call.MaxTime <= call.StartTime)
        throw new ArgumentException("MaxTime must be greater than StartTime.");

    if (!Enum.IsDefined(typeof(BO.BoCallType), call.CallType))
        throw new ArgumentException("Invalid CallType.");

    if (!Enum.IsDefined(typeof(BO.BoCallStatus), call.Status))
        throw new ArgumentException("Invalid CallStatus.");

    if (call.AssignInList != null)
    {
        foreach (var assign in call.AssignInList)
        {
            if (assign.VolunteerId <= 0)
                throw new ArgumentException("VolunteerId must be a positive integer.");

            if (string.IsNullOrWhiteSpace(assign.VolunteerName))
                throw new ArgumentException("VolunteerName cannot be null or empty.");

            if (assign.AssignTime == default)
                throw new ArgumentException("AssignTime must be a valid date.");

            if (assign.EndedTime.HasValue && assign.EndedTime <= assign.AssignTime)
                throw new ArgumentException("EndedTime must be greater than AssignTime.");

            if (assign.EndType.HasValue && !Enum.IsDefined(typeof(BO.BoAssignmentEndReason), assign.EndType))
                throw new ArgumentException("Invalid EndType.");
        }
    }

    return true;
}
    internal static DO.Call ConvertToDoCall(BO.Call call)
    {
        var doCall = new DO.Call
        {

            Type = (DO.CallType)call.CallType,
            Address = call.Address,
            Latitude = call.Latitude.Value,
            Longitude = call.Longitude.Value,
            StartTime = call.StartTime,
            Description = call.Description,
            MaxTime = call.MaxTime,
        };

        return doCall;
    }
    internal static void CheckStatus(BO.Call call) 
    {
        if (call == null)
        {
            throw new KeyNotFoundException("Call not found.");
        }
        if (call.Status != BO.BoCallStatus.Open || (call.AssignInList != null && call.AssignInList.Any()))
        {
            throw new InvalidOperationException("Cannot delete call. The call is either not open or has been assigned to a volunteer.");
        }
    }

    /// <summary>
    /// Gets the status of a call based on its ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The status of the call.</returns>
    /// <exception cref="BO.BlNotExistException">Thrown when the call is not found.</exception>
    internal static BO.BoCallStatus GetCallStatus(int callId)
    {
        DO.Call call = s_dal.Call.Read(callId) ?? throw new BO.BlNotExistException("Call not found");
        DO.Assignment? assignment = s_dal.Assignment.Read(a => a.CallId == callId);

        DateTime now = ClockManager.Now;
        DateTime? maxTime = s_dal.Call.Read(callId)!.MaxTime;

        if (maxTime is not null && now > maxTime) // call is overdue
            return BO.BoCallStatus.OverDated;

        else if (assignment is not null) // call is assigned
        {
            if (assignment.EndReason is not null) // call is closed
                return BO.BoCallStatus.Closed;

            else if (call.MaxTime is null) // call has no MaxTime
                return BO.BoCallStatus.InTreatment;

            else if (now < maxTime - s_dal.Config.RiskRange) // call is in treatment and not overdue but in the risk range
                return BO.BoCallStatus.InTreatmentAndDanger;

            else // call is in treatment and not overdue
                return BO.BoCallStatus.InTreatment;
        }

        if (call.MaxTime is null) // call is open and has no MaxTime
            return BO.BoCallStatus.Open;

        else if (now < maxTime - s_dal.Config.RiskRange) // call is open and not overdue but in the risk range
            return BO.BoCallStatus.OpenAndDanger;

        else // call is open and not overdue
            return BO.BoCallStatus.Open;
    }

    /// <summary>
    /// Updates the status of calls based on their MaxTime and current assignments.
    /// If a call is overdue and has no assignment, a new assignment is created with an OverDated status.
    /// If a call is overdue and has an assignment, the assignment is updated with an OverDated status.
    /// </summary>
    internal static void UpdateCallStatus()
    {
        var clock = ClockManager.Now;
        var overDatedCalls = from call in s_dal.Call.ReadAll()
                             let assign = s_dal.Assignment.Read(a => a.CallId == call.Id)
                             where call.MaxTime is not null && clock > call.MaxTime
                             group call by (assign == null) into g
                             select g;

        foreach (var callGroup in overDatedCalls)
        {
            foreach (var call in callGroup)
            {
                if (callGroup.Key is true) // call has no assignment
                {
                    s_dal.Assignment.Create(new DO.Assignment
                    {
                        CallId = call.Id,
                        VolunteerId = 0,
                        StartTime = clock,
                        EndTime = clock,
                        EndReason = DO.AssignmentEndReason.OverDated
                    });
                }
                else if (callGroup.Key is false) // call has an assignment
                {
                    DO.Assignment assignment = s_dal.Assignment.Read(a => a.CallId == call.Id)!;
                    s_dal.Assignment.Update(new DO.Assignment
                    {
                        CallId = call.Id,
                        VolunteerId = assignment.VolunteerId,
                        StartTime = assignment.StartTime,
                        EndTime = clock,
                        EndReason = DO.AssignmentEndReason.OverDated
                    });
                }
            }
        }
    }

}

//bool ValidateCall(BO.Call call)
//{
//    if (string.IsNullOrWhiteSpace(call.Address))
//        throw new ArgumentException("Address cannot be null or empty.");
//    if(call.Id)
//    if (call.Latitude < -90 || call.Latitude > 90)
//        throw new ArgumentException("Latitude must be between -90 and 90.");
//    if (call.Longitude < -180 || call.Longitude > 180)
//        throw new ArgumentException("Longitude must be between -180 and 180.");
//    if (call.StartTime == default)
//        throw new ArgumentException("StartTime must be a valid date.");
//    if (call.MaxTime.HasValue && call.MaxTime <= call.StartTime)
//        throw new ArgumentException("MaxTime must be greater than StartTime.");
//    return true;
//}



