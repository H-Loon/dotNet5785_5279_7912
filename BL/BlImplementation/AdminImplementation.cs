namespace BlImplementation;
using BlApi;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddToConfigClock(int value, BO.TimeType type)
    {
        throw new NotImplementedException();
    }

    public DateTime GetConfigClock()
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetRiskRange()
    {
        throw new NotImplementedException();
    }

    public void InitDB()
    {
        throw new NotImplementedException();
    }

    public void ResetDB()
    {
        throw new NotImplementedException();
    }

    public void UpdateRiskRange(TimeSpan riskRange)
    {
        throw new NotImplementedException();
    }
}
