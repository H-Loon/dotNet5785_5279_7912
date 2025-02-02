namespace BlApi;

public interface IVolunteer : IObservable //stage 5
{
    /// <summary>
    /// Authenticates a volunteer using their name and password.
    /// </summary>
    /// <param name="name">The name of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role type of the authenticated volunteer.</returns>
    BO.BoRoleType Login(string name, string password);

    /// <summary>
    /// Retrieves a list of s_volunteers based on their active status and a specified field.
    /// </summary>
    /// <param name="active">The active status of the s_volunteers to retrieve.</param>
    /// <param name="field">The field to sort the s_volunteers by.</param>
    /// <returns>A list of s_volunteers matching the specified criteria.</returns>
    IEnumerable<BO.VolunteerInList> GetVolunteerInList(bool? active, BO.VolunteerInListField? field);

    /// <summary>
    /// Retrieves a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The volunteer with the specified ID.</returns>
    BO.Volunteer GetVolunteer(int id);

    /// <summary>
    /// Updates the information of an existing volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer to update.</param>
    /// <param name="volunteer">The updated volunteer information.</param>
    void UpdateVolunteer(int id, BO.Volunteer volunteer);

    /// <summary>
    /// Deletes a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    void DeleteVolunteer(int id);

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    /// <param name="volunteer">The volunteer to add.</param>
    void AddVolunteer(BO.Volunteer volunteer);

    bool IsDeletable(int id);
}
