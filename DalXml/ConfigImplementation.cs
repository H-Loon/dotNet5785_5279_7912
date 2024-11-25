namespace Dal;
using DalApi;
using DO;
/// <summary>
/// Implementation of the IConfig interface for managing configuration settings.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the current clock value in the configuration.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock; // Retrieves the clock value from the Config class.
        set => Config.Clock = value; // Sets the clock value in the Config class.
    }

    /// <summary>
    /// Gets or sets the risk range time span in the configuration.
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange; // Retrieves the risk range value from the Config class.
        set => Config.RiskRange = value; // Sets the risk range value in the Config class.
    }

    /// <summary>
    /// Resets the configuration values to their default settings.
    /// </summary>
    public void Reset()
    {
        Config.Reset(); // Calls the Reset method in the Config class to reset values.
    }
}
