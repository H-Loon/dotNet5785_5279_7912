namespace BlApi;

public interface IAdmin
{
    DateTime GetConfigClock();
    void AddToConfigClock(int value, BO.TimeType type);
    TimeSpan GetRiskRange();
    void UpdateRiskRange(TimeSpan riskRange);
    void ResetDB();
    void InitDB();
}
