namespace BlImplementation;
using BlApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void ForwardClock(BO.TimeUnit type)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
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
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.InitializeDB(); //stage 7
        VolunteerManager.PasswordFillerForInit();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;
        VolunteerManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyListUpdated();
        AssignmentManager.Observers.NotifyListUpdated();
    }

    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;
        VolunteerManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyListUpdated();
        AssignmentManager.Observers.NotifyListUpdated();
    }

    public void UpdateRiskRange(TimeSpan riskRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
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

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
    => AdminManager.Stop(); //stage 7
}
