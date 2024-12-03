namespace BlApi;

public interface ICall
{
    int[] GetCallsQuantities();
    IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2);
    BO.Call GetCall(int id);
    void UpdateCall(BO.Call call);
    void DeleteCall(int id);
    void AddCall(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCallByVolunteer(int id, BO.BoCallType? boCallType, BO.ClosedCallInListField? field);
    IEnumerable<BO.OpenCallInList> GetOpenCallForVolunteer(int id, BO.BoCallType? boCallType, BO.OpenCallInListField? field);
    void CompleteCall(int volunteerId, int callId);
    void CancelCall(int cancelerId, int callId);
    void AssignCall(int volunteerId, int callId);
}
