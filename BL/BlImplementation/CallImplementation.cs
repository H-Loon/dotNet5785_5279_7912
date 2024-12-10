namespace BlImplementation;
using BlApi;
using System.Collections.Generic;
using Helpers;
using DO;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //private readonly List<BO.Call> _calls = new List<BO.Call>();//

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

        CallManager.ValidateCallFormat(call);
        CallManager.ValidateCallLogical(call);

        DO.Call dataCall = CallManager.ConvertToDoCall(call);

        try
        {
            _dal.Call.Create(dataCall);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("A call with the same ID already exists.", ex);
        }
    }

    public void DeleteCall(int id)
    {
        try
        {
            // Retrieve the call details
            var call = _dal.Call.Read(id) ?? throw new KeyNotFoundException("Call not found.");
            if(call == null)
        {
                throw new KeyNotFoundException("Call not found.");
            }
            if (CallManager.GetCallStatus(call.Id) != BO.BoCallStatus.Open || _dal.Assignment.Read(a => a.CallId == call.Id) != null)
            {
                throw new InvalidOperationException("Cannot delete call. The call is either not open or has been assigned to a volunteer.");
            }

            _dal.Call.Delete(id);
        }
        catch (KeyNotFoundException ex)
        {

            throw new InvalidOperationException("Call not found.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while trying to delete the call.", ex);
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
                              let call = _dal.Call.Read(assignment.CallId)
                              where call != null && (CallManager.GetCallStatus(call.Id)== BO.BoCallStatus.Closed)
                              select new BO.ClosedCallInList
                              {
                                  Id = assignment.CallId,
                                  CallType = (BO.BoCallType)call.Type,
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
                    BO.ClosedCallInListField.EndedTime => closedCalls.OrderBy(c => c.EndedTime),
                    BO.ClosedCallInListField.EndType => closedCalls.OrderBy(c => c.EndType),
                    _ => closedCalls.OrderBy(c => c.Id)
                };
            }
            else
            {
                closedCalls = closedCalls.OrderBy(c => c.Id);
            }

            return closedCalls.ToList();
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
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId) ?? throw new KeyNotFoundException("Assignments tot the Call not found.");

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
                AssignInList = assignments.Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    AssignTime = a.StartTime,
                    EndedTime = a.EndTime,
                    EndType = (BO.BoAssignmentEndReason?)a.EndReason
                }).ToList()
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

    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2) // filter by field1 and sort by field2
    {
        var assignments = _dal.Assignment.ReadAll();
        var calls = _dal.Call.ReadAll();

        IEnumerable<BO.CallInList> callInList = CallManager.GetCallInList();
            
        callInList = field1 switch
        {
            BO.CallInListField.AssignmentId => callInList.Where(c => c.AssignmentId == (int)obj!),
            BO.CallInListField.CallId => callInList.Where(c => c.CallId == (int)obj!),
            BO.CallInListField.CallType => callInList.Where(c => c.CallType == (BO.BoCallType)obj!),
            BO.CallInListField.StartTime => callInList.Where(c => c.StartTime == (DateTime)obj!),
            BO.CallInListField.TimeLeft => callInList.Where(c => c.TimeLeft == (TimeSpan)obj!),
            BO.CallInListField.LastVolunteerName => callInList.Where(c => c.LastVolunteerName == (string)obj!),
            BO.CallInListField.TimeOpen => callInList.Where(c => c.TimeOpen == (TimeSpan)obj!),
            BO.CallInListField.CallStatus => callInList.Where(c => c.CallStatus == (BO.BoCallStatus)obj!),
            BO.CallInListField.AssignCount => callInList.Where(c => c.AssignCount == (int)obj!),
            _ => callInList
        };
        callInList = field2 switch
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

    public IEnumerable<BO.ClosedCallInList> GetClosedCallByVolunteer(int id, BO.BoCallType? boCallType, BO.ClosedCallInListField? field)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2)
    {
        throw new NotImplementedException();
    }

    public void CompleteCall(int volunteerId, int assignmentId)
    {
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
        }
        catch (Exception ex)
        {
            throw new BO.BlCallCompletionException("An error occurred while trying to complete the call.", ex);
        }
    }

    public void CancelCall(int cancelerId, int assignmentId)
    {
        DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlNotExistException("Assignment not found.");
        var canceler = _dal.Volunteer.Read(cancelerId) ?? throw new BO.BlNotExistException("Volunteer not found.");

        if (assignment.VolunteerId != cancelerId && canceler.Role is not DO.RoleType.Admin)
            throw new BO.BlNotAllowedException("You are not authorized to cancel this call.");
        if (assignment.EndTime is not null)
            throw new BO.BlNotAllowedException("This call has already been ended.");
        var endReason = canceler.Role is DO.RoleType.Admin ? DO.AssignmentEndReason.CanceledByAdmin : DO.AssignmentEndReason.CanceledByVolunteer;
        try
        {
            _dal.Assignment.Update(assignment with
            {
                EndTime = DateTime.Now,
                EndReason = endReason
            });
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
        DO.Assignment assignment = _dal.Assignment.Read(a => a.CallId == callId) ?? throw new BO.BlNotAllowedException("You are not authorized to assign this call.");
        DO.Call call = _dal.Call.Read(callId) ?? throw new BO.BlNotExistException("Call not found.");
        DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotExistException("Volunteer not found.");
            
        if (CallManager.GetCallStatus(callId) is not BO.BoCallStatus.Open or BO.BoCallStatus.OpenAndDanger )
            throw new BO.BlNotAllowedException("You are not authorized to assign this call.");

        try
        {
            _dal.Assignment.Create(new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            throw new BO.BlCallAssignException("An error occurred while trying to assign the call.", ex);
        }
    }
}



