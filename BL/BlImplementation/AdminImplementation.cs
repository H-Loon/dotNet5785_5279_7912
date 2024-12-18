namespace BlImplementation;
using BlApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void ForwardClock(BO.TimeUnit type)
    {
        switch (type)
        {
            case BO.TimeUnit.Seconds:
                AdminManager.UpdateClock(AdminManager.Now.AddSeconds(1));
                break;
            case BO.TimeUnit.Minutes:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case BO.TimeUnit.Hours:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case BO.TimeUnit.Days:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case BO.TimeUnit.Months:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case BO.TimeUnit.Years:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
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
            return AdminManager.RiskRange;
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
        AdminManager.RiskRange = riskRange;
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
