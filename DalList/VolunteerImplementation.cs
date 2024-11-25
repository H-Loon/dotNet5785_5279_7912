namespace Dal;
using DalApi;
using DO;

/// <summary>
/// Implementation of the IVolunteer interface for managing Volunteer entities.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new volunteer.
    /// </summary>
    /// <param name="item">The volunteer to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown if a volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with Id ={item.Id} already exists ");
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Deletes a volunteer by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    /// <exception cref="DalNotExistException">Thrown if the volunteer with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        Volunteer volunteer = Read(id) ?? throw new DalNotExistException($"Assignment with Id ={id} doesn t exists");
        DataSource.Volunteers.Remove(volunteer);
    }

    /// <summary>
    /// Deletes all volunteers.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Reads a volunteer by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to read.</param>
    /// <returns>The volunteer with the specified ID, or null if not found.</returns>
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
    }

    /// <summary>
    /// Reads all volunteers, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all volunteers.</param>
    /// <returns>An enumerable of volunteers.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter is null ? DataSource.Volunteers : DataSource.Volunteers.Where(filter);
    }

    /// <summary>
    /// Updates an existing volunteer.
    /// </summary>
    /// <param name="item">The volunteer to update.</param>
    /// <exception cref="DalNotExistException">Thrown if the volunteer with the specified ID does not exist.</exception>
    public void Update(Volunteer item)
    {
        var existingVolunteer = Read(item.Id) ?? throw new DalNotExistException($"Volunteer with Id ={item.Id} doesn t exists");
        DataSource.Volunteers.Remove(existingVolunteer);
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Reads a volunteer by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The volunteer that matches the filter, or null if not found.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
