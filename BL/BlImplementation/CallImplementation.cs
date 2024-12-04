namespace BlImplementation;
using BlApi;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
        throw new NotImplementedException();
    }

    public void AssignCall(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void CancelCall(int cancelerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void CompleteCall(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Call GetCall(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallsQuantities()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallByVolunteer(int id, BO.BoCallType? boCallType, BO.ClosedCallInListField? field)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallForVolunteer(int id, BO.BoCallType? boCallType, BO.OpenCallInListField? field)
    {
        throw new NotImplementedException();
    }

    public void UpdateCall(BO.Call call)
    {
        throw new NotImplementedException();
    }
}

