namespace BlImplementation;
using BlApi;
/// <summary>
/// Represents the business logic layer implementation.
/// </summary>
internal class Bl : IBl
{
    /// <summary>
    /// Gets the admin implementation.
    /// </summary>
    public IAdmin Admin { get; } = new AdminImplementation();

    /// <summary>
    /// Gets the volunteer implementation.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Gets the call implementation.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();
}
