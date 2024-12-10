namespace BlApi;

public interface IAdmin
{
    /// <summary>
    /// Gets the current configuration clock.
    /// </summary>
    /// <returns>The current configuration clock as a DateTime.</returns>
    DateTime GetConfigClock();

    /// <summary>
    /// Advances the configuration clock by a specified value and time unit.
    /// </summary>
    /// <param name="value">The amount to advance the clock.</param>
    /// <param name="type">The unit of time to use for advancing the clock.</param>
    void ForwardClock(int value, BO.TimeUnit type);

    /// <summary>
    /// Gets the current risk range.
    /// </summary>
    /// <returns>The current risk range as a TimeSpan.</returns>
    TimeSpan GetRiskRange();

    /// <summary>
    /// Updates the risk range with a new value.
    /// </summary>
    /// <param name="riskRange">The new risk range to set.</param>
    void UpdateRiskRange(TimeSpan riskRange);

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    void ResetDB();

    /// <summary>
    /// Initializes the database.
    /// </summary>
    void InitDB();
}
