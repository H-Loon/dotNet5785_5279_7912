namespace Dal;
using DalApi;
using DO;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new call.
    /// </summary>
    /// <param name="item">The call to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a call with the same Id already exists.</exception>
    public void Create(Call item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Call with Id ={item.Id} already exists ");
        DataSource.Calls.Add(item with { Id = Config.NextCallId });
    }

    /// <summary>
    /// Deletes a call by Id.
    /// </summary>
    /// <param name="id">The Id of the call to delete.</param>
    /// <exception cref="DalNotExistException">Thrown when the call with the specified Id does not exist.</exception>
    public void Delete(int id)
    {
        var call = Read(id);
        if (call is null)
            throw new DalNotExistException($"Call with Id ={id} doesn't exist");
        DataSource.Calls.Remove(call);
    }

    /// <summary>
    /// Deletes all calls.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Reads a call by Id.
    /// </summary>
    /// <param name="id">The Id of the call to read.</param>
    /// <returns>The call with the specified Id, or null if not found.</returns>
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// Reads all calls, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all calls.</param>
    /// <returns>An enumerable of calls.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter == null ? DataSource.Calls : DataSource.Calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing call.
    /// </summary>
    /// <param name="item">The call to update.</param>
    /// <exception cref="DalNotExistException">Thrown when the call with the specified Id does not exist.</exception>
    public void Update(Call item)
    {
        var existingCall = Read(item.Id);
        if (existingCall is null)
            throw new DalNotExistException($"Call with Id ={item.Id} doesn't exist");
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Add(item);
    }

    /// <summary>
    /// Reads a call by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The call that matches the filter, or null if not found.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
