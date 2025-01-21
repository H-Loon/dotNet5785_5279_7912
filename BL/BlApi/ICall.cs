namespace BlApi;

public interface ICall : IObservable //stage 5
{
    /// <summary>
    /// Gets the quantities of calls.
    /// </summary>
    /// <returns>An array of integers representing the quantities of calls.</returns>
    int[] GetCallsQuantities();
    /// <summary>
    /// Gets a list of calls based on specified fields and an object.
    /// </summary>
    /// <param name="field1">The first field to filter the calls.</param>
    /// <param name="obj">The object to filter the calls.</param>
    /// <param name="field2">The second field to filter the calls.</param>
    /// <returns>An enumerable list of calls.</returns>
    IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2);
    /// <summary>
    /// Gets a call by its ID.
    /// </summary>
    /// <param name="callid">The ID of the call.</param>
    /// <returns>The call with the specified ID.</returns>
    BO.Call GetCall(int callid);
    /// <summary>
    /// Updates the specified call.
    /// </summary>
    /// <param name="call">The call to update.</param>
    void UpdateCall(BO.Call call);
    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to delete.</param>
    void DeleteCall(int id);
    /// <summary>
    /// Adds a new call.
    /// </summary>
    /// <param name="call">The call to add.</param>
    void AddCall(BO.Call call);
    /// <summary>
    /// Gets a list of closed calls for a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="boCallType">The type of the call.</param>
    /// <param name="field">The field to filter the calls.</param>
    /// <returns>An enumerable list of closed calls.</returns>
    IEnumerable<BO.ClosedCallInList> GetClosedCallByVolunteer(int id, BO.BoCallType? boCallType, BO.ClosedCallInListField? field);
    /// <summary>
    /// Gets a list of open calls for a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="boCallType">The type of the call.</param>
    /// <param name="field">The field to filter the calls.</param>
    /// <returns>An enumerable list of open calls.</returns>
    IEnumerable<BO.OpenCallInList> GetOpenCallForVolunteer(int id, BO.BoCallType? boCallType, BO.OpenCallInListField? field);
    /// <summary>
    /// Completes a call assignment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    void CompleteCall(int volunteerId, int assignmentId);
    /// <summary>
    /// Cancels a call assignment.
    /// </summary>
    /// <param name="cancelerId">The ID of the person canceling the call.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    void CancelCall(int cancelerId, int assignmentId);
    /// <summary>
    /// Assigns a call to a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to assign the call to.</param>
    /// <param name="callId">The ID of the call to be assigned.</param>
    void AssignCall(int volunteerId, int callId);
    /// <summary>
    /// Checks if a call is deletable.
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    bool IsDeletable(int callId);
}
