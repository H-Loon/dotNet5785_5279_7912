namespace BlImplementation;
using BlApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void ForwardClock(int value, BO.TimeUnit type)
    {
        switch (type)
        {
            case BO.TimeUnit.Seconds:
                ClockManager.UpdateClock(ClockManager.Now.AddSeconds(value));
                break;
            case BO.TimeUnit.Minutes:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(value));
                break;
            case BO.TimeUnit.Hours:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(value));
                break;
            case BO.TimeUnit.Days:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(value));
                break;
            case BO.TimeUnit.Weeks:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(value * 7));
                break;
            case BO.TimeUnit.Months:
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(value));
                break;
            case BO.TimeUnit.Years:
                ClockManager.UpdateClock(ClockManager.Now.AddYears(value));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public DateTime GetConfigClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetRiskRange()
    {
        return _dal.Config.RiskRange;
    }

    public void InitDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void UpdateRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }
}
