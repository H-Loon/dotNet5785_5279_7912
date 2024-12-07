namespace BlApi;

public interface IAdmin
{
    DateTime GetConfigClock();
    void ForwardClock(int value, BO.TimeUnit type);
    TimeSpan GetRiskRange();
    void UpdateRiskRange(TimeSpan riskRange);
    void ResetDB();
    void InitDB();
}
