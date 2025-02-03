using BO;
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
        lock (AdminManager.BlMutex) //stage 7
        {
            if (GetCallStatus(call.Id) != BO.BoCallStatus.Open || s_dal.Assignment.Read(a => a.CallId == call.Id) != null)
            {
                throw new InvalidOperationException("Cannot delete call. The call is either not open or has been assigned to a volunteer.");
            }
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
        DO.Call call;
        lock (AdminManager.BlMutex) //stage 7
            call = s_dal.Call.Read(callId) ?? throw new BO.BlNotExistException("Call not found");

        IEnumerable<DO.Assignment>? assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll(a => a.CallId == callId);

        DO.Assignment? assignment = assignments.LastOrDefault();

        DateTime now = AdminManager.Now;

        DateTime? maxTime;
        lock (AdminManager.BlMutex) //stage 7
            maxTime = s_dal.Call.Read(callId).MaxTime;

        TimeSpan riskRange;
        lock (AdminManager.BlMutex) //stage 7
            riskRange = s_dal.Config.RiskRange;

        if (maxTime is not null && now > maxTime) // call is overdue
            return BO.BoCallStatus.OverDated;

        else if (assignment is not null) // call is assigned
        {
            if (assignment.EndReason is DO.AssignmentEndReason.Completed) // call is closed
                return BO.BoCallStatus.Closed;
            
            if (assignment.EndReason is (DO.AssignmentEndReason.CanceledByAdmin or DO.AssignmentEndReason.CanceledByVolunteer) && now > maxTime - riskRange) // call is open and in danger because no in treatment and is in the risk range
                return BO.BoCallStatus.OpenAndDanger;

            if (assignment.EndReason is DO.AssignmentEndReason.CanceledByAdmin or DO.AssignmentEndReason.CanceledByVolunteer) // call is open because no in treatment
                return BO.BoCallStatus.Open;

            if (call.MaxTime is null) // call has no MaxTime
                return BO.BoCallStatus.InTreatment;

            if (now > maxTime - riskRange) // call is in treatment and not overdue but in the risk range
                return BO.BoCallStatus.InTreatmentAndDanger;

            else // call is in treatment and not overdue
                return BO.BoCallStatus.InTreatment;
        }

        if (call.MaxTime is null) // call is open and has no MaxTime
            return BO.BoCallStatus.Open;

        if (now > maxTime - riskRange) // call is open and not overdue but in the risk range
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
        bool flag = false;
        int aId = 0;
        var clock = AdminManager.Now;

        lock (AdminManager.BlMutex) //stage 7
        {
            var overDatedCalls = from call in s_dal.Call.ReadAll().ToList()
                                 let assign = s_dal.Assignment.Read(a => a.CallId == call.Id)
                                 let status = GetCallStatus(call.Id)
                                 where call.MaxTime is not null && (status != BO.BoCallStatus.Closed || status != BO.BoCallStatus.OverDated) && clock > call.MaxTime
                                 group call by (assign == null) into g
                             select g;
            foreach (var callGroup in overDatedCalls)
            {
                foreach (var call in callGroup)
                {
                    var status = GetCallStatus(call.Id);
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
                    else if (callGroup.Key is false && status != BO.BoCallStatus.Closed && status != BO.BoCallStatus.OverDated) // call has an assignment
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
                        aId = assignment.Id;
                        flag = true;
                    }
                }
            }
        }
        if(flag)
            Observers.NotifyItemUpdated(aId); //stage 5
        Observers.NotifyListUpdated(); //stage 5
    }

    internal static TimeSpan? TimeLeft(DO.Call call)
    {
        var status = GetCallStatus(call.Id);
        if (call.MaxTime is null)
            return null;
        if (status == BO.BoCallStatus.Closed || status == BO.BoCallStatus.OverDated)
        {
            return TimeSpan.Zero;
        }
        return call.MaxTime.Value - AdminManager.Now;
    }
    internal static TimeSpan? TimeOpen(DO.Call call)
    {
        var status = GetCallStatus(call.Id);

        if(status == BO.BoCallStatus.Closed || status == BO.BoCallStatus.OverDated)
        {
            var assignment = s_dal.Assignment.ReadAll(a=> a.CallId == call.Id).ToList().Last();
            return assignment.EndTime - call.StartTime;
        }

        return AdminManager.Now - call.StartTime;
    }

    internal static IEnumerable<BO.CallInList> GetCallInList()
    {
        IEnumerable<DO.Call> calls;
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
            calls = s_dal.Call.ReadAll().ToList();
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll().ToList();

        IEnumerable<BO.CallInList> callInList;
        lock (AdminManager.BlMutex) //stage 7
        {
            callInList = from call in calls
            let assignment = assignments.LastOrDefault(a => a.CallId == call.Id)
            let assignmentId = assignment?.Id ?? 0
            let volunteerId = assignment?.VolunteerId ?? 0
            let EndedTime = assignment?.EndTime
            let assignCount = assignments.Count(a => a.CallId == call.Id)
            let callStatus = GetCallStatus(call.Id)
            select new BO.CallInList
            {
                AssignmentId = assignmentId is 0 ? null : assignmentId,
                CallId = call.Id,
                CallType = (BO.BoCallType)call.Type,
                StartTime = call.StartTime,
                TimeLeft = CallManager.TimeLeft(call),
                LastVolunteerName = s_dal.Volunteer.Read(volunteerId)?.Name ?? null,
                TimeOpen = CallManager.TimeOpen(call),
                CallStatus = callStatus,
                AssignCount = assignCount
            };
            callInList = callInList.ToList();
        }
        return callInList;
    }

    internal static async Task UpdateCoordonates(BO.Call call)
    {
        (call.Latitude, call.Longitude) = await Tools.AddressToCoordinatesAsync(call.Address);
    }

    internal static IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.BoCallType? boCallType, BO.OpenCallInListField? field = BO.OpenCallInListField.Id)
    {
        try
        {
            IEnumerable<BO.OpenCallInList>? openCalls;
            lock (AdminManager.BlMutex)//stage 7
            {
                // Retrieve all calls 
                var allCalls = s_dal.Call.ReadAll();
                var volunteer = s_dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotExistException("Volunteer not found.");

                // Filter calls with status "Open" or "OpenAndDanger"
                openCalls = from openCall in allCalls
                            let status = CallManager.GetCallStatus(openCall.Id)
                            let callType = (BO.BoCallType)openCall.Type
                            let distance = Tools.GetCallDistance(openCall.Id, volunteer)
                            where (status == BO.BoCallStatus.Open || status == BO.BoCallStatus.OpenAndDanger) &&
                                  (boCallType is null || callType == boCallType) && distance <= volunteer.MaxDistance
                            select new BO.OpenCallInList
                            {
                                Id = openCall.Id,
                                CallType = callType,
                                Description = openCall.Description,
                                Address = openCall.Address,
                                StartTime = openCall.StartTime,
                                MaxTime = openCall.MaxTime,
                                CallDistance = distance
                            };
                openCalls = openCalls.ToList();
            }

            // Order by the specified field
            if (field.HasValue)
            {
                openCalls = field.Value switch
                {
                    BO.OpenCallInListField.CallType => openCalls.OrderBy(c => c.CallType),
                    BO.OpenCallInListField.Description => openCalls.OrderBy(c => c.Description),
                    BO.OpenCallInListField.Address => openCalls.OrderBy(c => c.Address),
                    BO.OpenCallInListField.StartTime => openCalls.OrderBy(c => c.StartTime),
                    BO.OpenCallInListField.MaxTime => openCalls.OrderBy(c => c.MaxTime),
                    BO.OpenCallInListField.CallDistance => openCalls.OrderBy(c => c.CallDistance),
                    _ => openCalls.OrderBy(c => c.Id)
                };
            }
            else
            {
                openCalls = openCalls.OrderBy(c => c.Id);
            }

            return openCalls;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while trying to retrieve open calls for the volunteer.", ex);
        }
    }
    internal static void CompleteCall(int volunteerId, int assignmentId)
    {
        DO.Assignment assignment;
        lock (AdminManager.BlMutex) //stage 7
        {
            if (s_dal.Volunteer.Read(volunteerId) is null) throw new BO.BlNotExistException("Volunteer/Admin not found.");
            assignment = s_dal.Assignment.Read(assignmentId) ?? throw new BO.BlNotExistException("Assignment not found.");
            if (assignment.VolunteerId != volunteerId)
                throw new BO.BlNotAllowedException("You are not authorized to complete this call.");
            if (assignment.EndTime is not null)
                throw new BO.BlNotAllowedException("This call has already been ended.");
        }
        try
        {
            if (assignment.EndTime is not null)
            {
                return;
            }
            lock (AdminManager.BlMutex)
            {
                s_dal.Assignment.Update(assignment with
                {
                    EndTime = DateTime.Now,
                    EndReason = DO.AssignmentEndReason.Completed
                });
            }
        }
        catch (Exception ex)
        {
            throw new BO.BlCallCompletionException("An error occurred while trying to complete the call.", ex);
        }
        AssignmentManager.Observers.NotifyItemUpdated(assignmentId);  //stage 5
        Observers.NotifyListUpdated();  //stage 5
        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //stage 5
        VolunteerManager.Observers.NotifyListUpdated();
    }

    internal static void CancelCall(int cancelerId, int assignmentId)
    {
        try
        {
            DO.Assignment assignment;
            DO.Volunteer canceler;
            lock (AdminManager.BlMutex) //stage 7
            {
                assignment = s_dal.Assignment.Read(assignmentId) ?? throw new BO.BlNotExistException("Assignment not found.");
                canceler = s_dal.Volunteer.Read(cancelerId) ?? throw new BO.BlNotExistException("Volunteer not found.");
            }

            if (assignment.VolunteerId != cancelerId && canceler.Role is not DO.RoleType.Admin)
                throw new BO.BlNotAllowedException("You are not authorized to cancel this call.");
            if (assignment.EndTime is not null)
                throw new BO.BlNotAllowedException("This call has already been ended.");

            string receiver;

            lock (AdminManager.BlMutex) //stage 7
            {
                var endReason = canceler.Role is DO.RoleType.Admin ? DO.AssignmentEndReason.CanceledByAdmin : DO.AssignmentEndReason.CanceledByVolunteer;
                s_dal.Assignment.Update(assignment with
                {
                    EndTime = DateTime.Now,
                    EndReason = endReason
                });
                receiver = s_dal.Volunteer.Read(assignment.VolunteerId)?.Email ?? throw new BO.BlNotExistException("Volunteer not found.");
            }

            string msg = $"The call has been canceled by {canceler.Name}.";
            Tools.SendEmail("noreply@weirdaid.com", receiver, $"Call n.{assignment.CallId} has been canceled ", msg);

            AssignmentManager.Observers.NotifyItemUpdated(assignmentId);  //stage 5
            Observers.NotifyListUpdated();  //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new BO.BlCallCancelException("An error occurred while trying to cancel the call.", ex);
        }
    } 

    internal static void AssignCall(int volunteerId, int callId)
    {
        try
        {
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)//stage 7
            {
                if (s_dal.Call.Read(callId) is null) throw new BO.BlNotExistException("Call not found.");
                volunteer = s_dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotExistException("Volunteer not found.");
            }

            if (VolunteerManager.ConvertToBO(volunteerId).CurrentCall is not null) throw new BO.BlNotExistException("Volunteer has already a call assigned.");

            if (CallManager.GetCallStatus(callId) is not (BO.BoCallStatus.Open or BO.BoCallStatus.OpenAndDanger)) // call is not open or open and danger
                throw new BO.BlNotAllowedException("This call has already been assigned to a volunteer or is over dated.");

            if (volunteer.IsActive is false)
                throw new BO.BlNotAllowedException("The Volunteer is not active.");

            if (CallManager.GetCallStatus(callId) != BO.BoCallStatus.Open && CallManager.GetCallStatus(callId) != BO.BoCallStatus.OpenAndDanger)
                throw new BO.BlNotAllowedException("You are not authorized to assign this call.");

            lock (AdminManager.BlMutex)//stage 7
            {
                s_dal.Assignment.Create(new DO.Assignment
                {
                    CallId = callId,
                    VolunteerId = volunteerId,
                    StartTime = DateTime.Now
                });
            }
            Observers.NotifyListUpdated();  //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new BO.BlCallAssignException("An error occurred while trying to assign the call. ", ex);
        }
    }
}


