namespace BlImplementation;
using BlApi;
using System.Collections.Generic;
using Helpers;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //private readonly List<BO.Call> _calls = new List<BO.Call>();//


    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="call">The call object containing updated details.</param>
    /// <exception cref="InvalidOperationException">Thrown when the call is not found or an error occurs during the update.</exception>
    public void UpdateCall(BO.Call call)
    {
        CallManager.ValidateCallFormat(call);
        CallManager.ValidateCallLogical(call);

        DO.Call dataCall = CallManager.ConvertToDoCall(call);

        try
        {
            _dal.Call.Update(dataCall);
        }
        catch (KeyNotFoundException ex)
        {
            //if Id doesn't exist
            throw new InvalidOperationException("Call not found.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while trying to update the call.", ex);
        }
    }

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="call">The call object to be added.</param>
    /// <exception cref="InvalidOperationException">Thrown when a call with the same ID already exists or an error occurs during the addition.</exception>
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

    public int[] GetCallsQuantities()
    {
        // Lire tous les appels depuis la couche de données
        var calls = _dal.Call.ReadAll();

        // Grouper les appels par statut et compter le nombre d'appels dans chaque groupe
        var quantities = calls
            .GroupBy(c => (int)c.Status)
            .OrderBy(g => g.Key)
            .Select(g => g.Count())
            .ToArray();

        return quantities;
    }
    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    /// <exception cref="InvalidOperationException">Thrown when the call is not found or an error occurs during the deletion.</exception>
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

            // Filter calls with status "Open" or "OpenAndDanger"
            return  from openCall in allCalls
                            let status = CallManager.GetCallStatus(openCall.Id)
                            let callType = (BO.BoCallType)openCall.Type
                            where (status == BO.BoCallStatus.Open || status == BO.BoCallStatus.OpenAndDanger) && 
                                  (boCallType is null || callType == boCallType)
                            orderby openCall.GetType().GetProperty(field.ToString()!)
                            select new BO.OpenCallInList 
                            {
                                Id = openCall.Id,
                                CallType = callType,
                                Description = openCall.Description,
                                Address = openCall.Address,
                                StartTime = openCall.StartTime,
                                MaxTime = openCall.MaxTime,
                                CallDistance = Tools.GetCallDistance(openCall.Id,volunteer)
                            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while trying to retrieve open calls for the volunteer.", ex);
        }
    }

    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2)
    {
        throw new NotImplementedException();
    }

    public void CompleteCall(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void CancelCall(int cancelerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void AssignCall(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }
}



