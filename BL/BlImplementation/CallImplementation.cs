namespace BlImplementation;
using BlApi;
using System.Collections.Generic;
using Helpers;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Stage 5
    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    public int[] GetCallsQuantities()
    {
        var callsGroup = _dal.Call.ReadAll().GroupBy(c => CallManager.GetCallStatus(c.Id)).Select(g => new { Status = g.Key, Count = g.Count() });

        int[] callQuantitieByStatus = new int[Enum.GetValues(typeof(BO.BoCallStatus)).Length];
        foreach (var group in callsGroup)
        {
            callQuantitieByStatus[(int)group.Status] = group.Count;
        }
        return callQuantitieByStatus;
    }
    public void AddCall(BO.Call call)
    {
            AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            CallManager.ValidateCallFormat(call);
            CallManager.ValidateCallLogical(call);

            (call.Latitude, call.Longitude) = Tools.AddressToCoordinates(call.Address);

            DO.Call dataCall = CallManager.ConvertToDoCall(call);
            _dal.Call.Create(dataCall);
            CallManager.Observers.NotifyListUpdated();  //stage 5
            var callId = _dal.Call.ReadAll().Last().Id;
            var volunteers = from v in _dal.Volunteer.ReadAll()
                             where v.MaxDistance == null || v.MaxDistance >= Tools.GetCallDistance(callId, v)
                             select v;

            string msg = "A new call has been added to the system and in your range.\n Please check the system for more details.";
            foreach (var vol in volunteers)
            {
                Tools.SendEmail("noreply@weirdaid.com", vol.Email, $"New Call n.{callId} has been open and is in your range ", msg);
            }

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public void DeleteCall(int id)
    {
            AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            // Retrieve the call details
            var call = _dal.Call.Read(id) ?? throw new KeyNotFoundException("Call not found.");
            if(call == null)
                throw new BO.BlNotExistException("Call not found.");

            if (!IsDeletable(call.Id))
            {
                throw new BO.BlNotAllowedException("Cannot delete call. The call is either not open or has been assigned to a volunteer.");
            }

            _dal.Call.Delete(id);
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while trying to delete the call.", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of closed calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="boCallType">The type of call to filter by (optional).</param>
    /// <param name="field">The field to sort the results by (optional).</param>
    /// <returns>A list of closed calls associated with the volunteer.</returns>
    /// <exception cref="BO.BlNotExistException">Thrown when an error occurs while retrieving the closed calls.</exception>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallByVolunteer(int volunteerId, BO.BoCallType? boCallType, BO.ClosedCallInListField? field)
    {
        try
        {
            // Retrieve all assignments related to the volunteer
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndReason != null);

            // Filter only the closed calls associated
            var closedCalls = from assignment in assignments
                              let call = _dal.Call.Read(assignment.CallId)!
                              let callType = (BO.BoCallType)call!.Type
                              where boCallType is null || callType == boCallType
                              select new BO.ClosedCallInList
                              {
                                  Id = assignment.CallId,
                                  CallType = callType,
                                  Address = call.Address,
                                  StartTime = call.StartTime,
                                  AssignTime = assignment.StartTime,
                                  EndedTime = assignment.EndTime ?? default,
                                  EndType = (BO.BoAssignmentEndReason)assignment.EndReason!
                              };

            // Sort by the specified field
            if (field.HasValue)
            {
                closedCalls = field.Value switch
                {
                    BO.ClosedCallInListField.CallType => closedCalls.OrderBy(c => c.CallType),
                    BO.ClosedCallInListField.Address => closedCalls.OrderBy(c => c.Address),
                    BO.ClosedCallInListField.StartTime => closedCalls.OrderBy(c => c.StartTime),
                    BO.ClosedCallInListField.AssignTime => closedCalls.OrderBy(c => c.AssignTime),
                    BO.ClosedCallInListField.EndedTime => closedCalls.OrderBy(c => c.EndedTime),
                    BO.ClosedCallInListField.EndType => closedCalls.OrderBy(c => c.EndType),
                    _ => closedCalls.OrderBy(c => c.Id)
                };
            }
            else
            {
                closedCalls = closedCalls.OrderBy(c => c.Id);
            }

            return closedCalls;
        }
        catch (DO.DalNotExistException ex)
        {
            throw new BO.BlNotExistException($"Error retrieving closed calls for volunteer {volunteerId}: {ex.Message}", ex);
        }
    }
    /// <summary>
    /// Retrieves the details of a specific call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>The call object with the specified ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the call is not found or an error occurs during the retrieval.</exception>
    public BO.Call GetCall(int callId)
    {
        try
        {
            // Retrieve the call details
            var dataCall = _dal.Call.Read(callId) ?? throw new KeyNotFoundException("Call not found.");

            // Retrieve the list of assignments related to the call
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
            if (assignments.Count() == 0)
            {
                assignments = null;
            }

            // Construct the BO.Call object
            var boCall = new BO.Call
            {
                Id = dataCall.Id,
                CallType = (BO.BoCallType)dataCall.Type,
                Description = dataCall.Description,
                Address = dataCall.Address,
                Latitude = dataCall.Latitude,
                Longitude = dataCall.Longitude,
                StartTime = dataCall.StartTime,
                MaxTime = dataCall.MaxTime,
                Status = CallManager.GetCallStatus(dataCall.Id),
                AssignInList = assignments?.Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.Name ?? "",
                    AssignTime = a.StartTime,
                    EndedTime = a.EndTime,
                    EndType = (BO.BoAssignmentEndReason?)a.EndReason
                }).ToList() ?? null
            };

            return boCall;
        }
        catch (KeyNotFoundException ex)
        {
            // If the call with the given ID does not exist, throw an appropriate exception
            throw new InvalidOperationException("Call not found.", ex);
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            throw new InvalidOperationException("An error occurred while trying to retrieve the call details.", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of open calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="boCallType">The type of call to filter by (optional).</param>
    /// <param name="field">The field to sort the results by (optional).</param>
    /// <returns>A list of open calls associated with the volunteer.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs while retrieving the open calls.</exception>
    public IEnumerable<BO.OpenCallInList> GetOpenCallForVolunteer(int volunteerId, BO.BoCallType? boCallType, BO.OpenCallInListField? field = BO.OpenCallInListField.Id)
    {
        try
        {
            // Retrieve all calls 
            var allCalls = _dal.Call.ReadAll();
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotExistException("Volunteer not found.");

            // Filter calls with status "Open" or "OpenAndDanger"
            var openCalls = from openCall in allCalls
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

    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? filterField, object? obj, BO.CallInListField? sortedField) // filter by filterField and sort by sortedField
    {
        var assignments = _dal.Assignment.ReadAll();
        var calls = _dal.Call.ReadAll();

        IEnumerable<BO.CallInList> callInList = CallManager.GetCallInList();
            
        callInList = filterField switch
        {
            BO.CallInListField.AssignmentId => callInList.Where(c => obj == null || c.AssignmentId.ToString().Contains((string)obj!)),
            BO.CallInListField.CallId => callInList.Where(c => obj == null || c.CallId.ToString().Contains((string)obj!)),
            BO.CallInListField.CallType => callInList.Where(c => obj == null || ((BO.BoCallType)obj! == BO.BoCallType.None || c.CallType == (BO.BoCallType)obj!)),
            BO.CallInListField.StartTime => callInList.Where(c => obj == null || c.StartTime >= (DateTime)obj!),
            BO.CallInListField.TimeLeft => callInList.Where(c => obj == null || c.TimeLeft >= (TimeSpan)obj!),
            BO.CallInListField.LastVolunteerName => callInList.Where(c => { if (obj == null) return true; if(c.LastVolunteerName is null) return false; return c.LastVolunteerName.Contains((string)obj!); }),
            BO.CallInListField.TimeOpen => callInList.Where(c => obj == null || c.TimeOpen >= (TimeSpan)obj!),
            BO.CallInListField.CallStatus => callInList.Where(c => obj == null || c.CallStatus == (BO.BoCallStatus)obj),
            BO.CallInListField.AssignCount => callInList.Where(c => obj == null || c.AssignCount == (int)obj!),
            _ => callInList
        };
        callInList = sortedField switch
        {
            BO.CallInListField.AssignmentId => callInList.OrderBy(c => c.AssignmentId),
            BO.CallInListField.CallId => callInList.OrderBy(c => c.CallId),
            BO.CallInListField.CallType => callInList.OrderBy(c => c.CallType),
            BO.CallInListField.StartTime => callInList.OrderBy(c => c.StartTime),
            BO.CallInListField.TimeLeft => callInList.OrderBy(c => c.TimeLeft),
            BO.CallInListField.LastVolunteerName => callInList.OrderBy(c => c.LastVolunteerName),
            BO.CallInListField.TimeOpen => callInList.OrderBy(c => c.TimeOpen),
            BO.CallInListField.CallStatus => callInList.OrderBy(c => c.CallStatus),
            BO.CallInListField.AssignCount => callInList.OrderBy(c => c.AssignCount),
            _ => callInList.OrderBy(c => c.AssignmentId)
        };
        return callInList;
    }

    public void CompleteCall(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        if (_dal.Volunteer.Read(volunteerId) is null) throw new BO.BlNotExistException("Volunteer/Admin not found.");
        DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlNotExistException("Assignment not found.");
        if (assignment.VolunteerId != volunteerId)
            throw new BO.BlNotAllowedException("You are not authorized to complete this call.");
        if (assignment.EndTime is not null)
            throw new BO.BlNotAllowedException("This call has already been ended.");
        try
        {
            _dal.Assignment.Update(assignment with
            {
                EndTime = DateTime.Now,
                EndReason = DO.AssignmentEndReason.Completed
            });
            AssignmentManager.Observers.NotifyItemUpdated(assignmentId);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            throw new BO.BlCallCompletionException("An error occurred while trying to complete the call.", ex);
        }
    }

    public void CancelCall(int cancelerId, int assignmentId)
    {
            AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlNotExistException("Assignment not found.");
            var canceler = _dal.Volunteer.Read(cancelerId) ?? throw new BO.BlNotExistException("Volunteer not found.");

            if (assignment.VolunteerId != cancelerId && canceler.Role is not DO.RoleType.Admin)
                throw new BO.BlNotAllowedException("You are not authorized to cancel this call.");
            if (assignment.EndTime is not null)
                throw new BO.BlNotAllowedException("This call has already been ended.");
            var endReason = canceler.Role is DO.RoleType.Admin ? DO.AssignmentEndReason.CanceledByAdmin : DO.AssignmentEndReason.CanceledByVolunteer;
            _dal.Assignment.Update(assignment with
            {
                EndTime = DateTime.Now,
                EndReason = endReason
            });
            AssignmentManager.Observers.NotifyItemUpdated(assignmentId);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
            string msg = $"The call has been canceled by {canceler.Name}.";
            string receiver = _dal.Volunteer.Read(assignment.VolunteerId)?.Email ?? throw new BO.BlNotExistException("Volunteer not found.");
            Tools.SendEmail("noreply@weirdaid.com",receiver, $"Call n.{assignment.CallId} has been canceled " , msg);
        }
        catch (Exception ex)
        {
            throw new BO.BlCallCancelException("An error occurred while trying to cancel the call.", ex);
        }
    }

    public void AssignCall(int volunteerId, int callId)
    {
            AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            if(_dal.Call.Read(callId) is null) throw new BO.BlNotExistException("Call not found.");

            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotExistException("Volunteer not found.");

            if(VolunteerManager.ConvertToBO(volunteerId).CurrentCall is not null) throw new BO.BlNotExistException("Volunteer has already a call assigned.");
            
            if (CallManager.GetCallStatus(callId) is not (BO.BoCallStatus.Open or BO.BoCallStatus.OpenAndDanger)) // call is not open or open and danger
                throw new BO.BlNotAllowedException("This call has already been assigned to a volunteer or is over dated.");

            if (volunteer.IsActive is false)
                throw new BO.BlNotAllowedException("The Volunteer is not active.");  

            if (CallManager.GetCallStatus(callId) != BO.BoCallStatus.Open && CallManager.GetCallStatus(callId) != BO.BoCallStatus.OpenAndDanger )
                throw new BO.BlNotAllowedException("You are not authorized to assign this call.");

            _dal.Assignment.Create(new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = DateTime.Now
            });
            CallManager.Observers.NotifyListUpdated();  //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
        }
        catch (Exception ex)
        {
            throw new BO.BlCallAssignException("An error occurred while trying to assign the call. ", ex);
        }
    }

    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="call">The call object containing updated details.</param>
    /// <exception cref="InvalidOperationException">Thrown when the call is not found or an error occurs during the update.</exception>
    public void UpdateCall(BO.Call call)
    {
            AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            CallManager.ValidateCallFormat(call);
            CallManager.ValidateCallLogical(call);
            (call.Latitude, call.Longitude) = Tools.AddressToCoordinates(call.Address);

            DO.Call dataCall = CallManager.ConvertToDoCall(call);

            _dal.Call.Update(dataCall);
            CallManager.Observers.NotifyItemUpdated(call.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while trying to update the call.", ex);
        }
    }
    public bool IsDeletable(int callId) 
    { 
        return _dal.Assignment.Read(a => a.CallId == callId) == null && CallManager.GetCallStatus(callId) == BO.BoCallStatus.Open;
    }
}