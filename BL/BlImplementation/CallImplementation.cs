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

        // Création de l'objet DO.Call à partir de l'objet BO.Call
        CallManager.ValidateCall(call);

        DO.Call dataCall = CallManager.ConvertToDoCall(call);

        try
        {
            // Tentative d'ajout de la nouvelle appel à la couche de données
            _dal.Call.Create(dataCall);
        }
        catch (Exception ex)
        {
            // Capture de l'exception et relance d'une exception appropriée vers la couche de présentation
            throw new InvalidOperationException("A call with the same ID already exists.", ex);
        }
    }

    public void DeleteCall(int id)
    {
        try
        {
            var call = _dal.Call.Read(id);

            //CallManager.CheckStatus(call);

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
    public void UpdateCall(BO.Call call)
    {

        throw new NotImplementedException();

    }






    public BO.Call GetCall(int callid)
    {
        throw new NotImplementedException();
    }

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

    public IEnumerable<BO.OpenCallInList> GetOpenCallForVolunteer(int id, BO.BoCallType? boCallType, BO.OpenCallInListField? field)
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


