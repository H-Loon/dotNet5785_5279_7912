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
                AdminManager.UpdateClock(AdminManager.Now.AddSeconds(value));
                break;
            case BO.TimeUnit.Minutes:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(value));
                break;
            case BO.TimeUnit.Hours:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(value));
                break;
            case BO.TimeUnit.Days:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(value));
                break;
            case BO.TimeUnit.Weeks:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(value * 7));
                break;
            case BO.TimeUnit.Months:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(value));
                break;
            case BO.TimeUnit.Years:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(value));
                break;
        }
    }

    public DateTime GetConfigClock()
    {
        return AdminManager.Now;
    }

    public TimeSpan GetRiskRange()
    {
        try 
        { 
            return _dal.Config.RiskRange;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void InitDB()
    {
        DalTest.Initialization.Do();
        VolunteerManager.PasswordFillerForInit();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;
    }

    public void ResetDB()
    {
        _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;
    }

    public void UpdateRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
}
