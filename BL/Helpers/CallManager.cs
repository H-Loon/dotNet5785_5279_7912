using DalApi;
using DO;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 
    internal static void ValidateCallFormat(BO.Call call)
    {
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new ArgumentException("Address cannot be null or empty.");

        if (call.Description != null && call.Description.Length > 500)
            throw new ArgumentException("Description cannot be longer than 500 characters.");
    }
    internal static void ValidateCallLogical(BO.Call call)
    {
        if (call.MaxTime.HasValue && call.MaxTime <= call.StartTime)
            throw new ArgumentException("MaxTime must be greater than StartTime.");

        if(Tools.AddressCheck(call.Address) == false)
            throw new ArgumentException("Address is not valid.");
    }

    internal static DO.Call ConvertToDoCall(BO.Call call)
    {

        return new DO.Call
        {
            Id = call.Id,
            Type = (DO.CallType)call.CallType,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            StartTime = call.StartTime,
            Description = call.Description,
            MaxTime = call.MaxTime,
        };
    }
    internal static void CheckStatus(DO.Call call)
    {
        if (call == null)
        {
            throw new KeyNotFoundException("Call not found.");
        }
        if (GetCallStatus(call.Id) != BO.BoCallStatus.Open || s_dal.Assignment.Read(a => a.CallId == call.Id) != null)
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
        var assignments = s_dal.Assignment.ReadAll(a => a.CallId == callId);
        DO.Assignment? assignment = assignments.LastOrDefault();

        DateTime now = AdminManager.Now;
        DateTime? maxTime = s_dal.Call.Read(callId)!.MaxTime;

        if (maxTime is not null && now > maxTime) // call is overdue
            return BO.BoCallStatus.OverDated;

        else if (assignment is not null) // call is assigned
        {
            if (assignment.EndReason is DO.AssignmentEndReason.Completed) // call is closed
                return BO.BoCallStatus.Closed;

            if (assignment.EndReason is (DO.AssignmentEndReason.CanceledByAdmin or DO.AssignmentEndReason.CanceledByVolunteer) && now > maxTime - s_dal.Config.RiskRange) // call is open and in danger because no in treatment and is in the risk range
                return BO.BoCallStatus.OpenAndDanger;

            if (assignment.EndReason is DO.AssignmentEndReason.CanceledByAdmin or DO.AssignmentEndReason.CanceledByVolunteer) // call is open because no in treatment
                return BO.BoCallStatus.Open;

            else if (call.MaxTime is null) // call has no MaxTime
                return BO.BoCallStatus.InTreatment;

            else if (now > maxTime - s_dal.Config.RiskRange) // call is in treatment and not overdue but in the risk range
                return BO.BoCallStatus.InTreatmentAndDanger;

            else // call is in treatment and not overdue
                return BO.BoCallStatus.InTreatment;
        }

        if (call.MaxTime is null) // call is open and has no MaxTime
            return BO.BoCallStatus.Open;

        else if (now > maxTime - s_dal.Config.RiskRange) // call is open and not overdue but in the risk range
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
        var clock = AdminManager.Now;
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
                    Observers.NotifyListUpdated(); //stage 5
                }
                else if (callGroup.Key is false) // call has an assignment
                {
                    DO.Assignment assignment = s_dal.Assignment.Read(a => a.CallId == call.Id)!;
                    s_dal.Assignment.Update(new DO.Assignment
                    {
                        Id = assignment.Id,
                        CallId = call.Id,
                        VolunteerId = assignment.VolunteerId,
                        StartTime = assignment.StartTime,
                        EndTime = clock,
                        EndReason = DO.AssignmentEndReason.OverDated
                    });
                    Observers.NotifyItemUpdated(assignment.Id); //stage 5
                }
            }
        }
    }

    internal static TimeSpan? TimeLeft(DO.Call call)
    {
        if (call.MaxTime is null)
            return null;
        TimeSpan timeZero = new TimeSpan(0, 0, 0);
        TimeSpan timeLeft = call.MaxTime.Value - AdminManager.Now;
        return ( timeLeft > timeZero) ? timeLeft : timeZero;
    }
    internal static TimeSpan? TimeOpen(DO.Call call)
    {
        if (call.MaxTime is null)
            return null;
        if( GetCallStatus(call.Id) != BO.BoCallStatus.Closed && GetCallStatus(call.Id) != BO.BoCallStatus.OverDated)
            return null;
        TimeSpan timeOpen = AdminManager.Now - call.StartTime;
        return timeOpen;
    }

    internal static IEnumerable<BO.CallInList> GetCallInList()
    {
        var calls = s_dal.Call.ReadAll();
        var assignments = s_dal.Assignment.ReadAll();

        return from call in calls
        let assignment = assignments.LastOrDefault(a => a.CallId == call.Id)
        let assignmentId = assignment?.Id ?? 0
        let volunteerId = assignment?.VolunteerId ?? 0
        let EndedTime = assignment?.EndTime
        let assignCount = assignments.Count(a => a.CallId == call.Id)
        select new BO.CallInList
        {
            AssignmentId = assignmentId is 0 ? null : assignmentId,
            CallId = call.Id,
            CallType = (BO.BoCallType)call.Type,
            StartTime = call.StartTime,
            TimeLeft = CallManager.TimeLeft(call),
            LastVolunteerName = s_dal.Volunteer.Read(volunteerId)?.Name ?? null,
            TimeOpen = CallManager.TimeOpen(call),
            CallStatus = CallManager.GetCallStatus(call.Id),
            AssignCount = assignCount
        };
    }
}


