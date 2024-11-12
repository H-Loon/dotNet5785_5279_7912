namespace Dal;
using DalApi;
using DO;
public class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock; // qd on demande l'h , on va la chercher ds Config
        set => Config.Clock = value; // on modifie l'h ds Config ; la new heure sera value 
    }
    public TimeSpan RiskRange 
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public void Reset()
    {
        Config.Reset();
    }
}
