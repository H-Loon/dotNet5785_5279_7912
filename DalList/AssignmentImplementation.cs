namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="item">The assignment to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when an assignment with the same Id already exists.</exception>
    public void Create(Assignment item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Assignment with Id ={item.Id} already exists ");
        DataSource.Assignments.Add(item with { Id = Config.NextAssignmentId });
    }

    /// <summary>
    /// Deletes an assignment by Id.
    /// </summary>
    /// <param name="id">The Id of the assignment to delete.</param>
    /// <exception cref="DalNotExistException">Thrown when the assignment with the specified Id does not exist.</exception>
    public void Delete(int id)
    {
        var assignment = Read(id);
        if (assignment is null)
            throw new DalDeletionImpossibleException($"Assignment with Id ={id} doesn't exist");
        DataSource.Assignments.Remove(assignment);
    }

    /// <summary>
    /// Deletes all assignments.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Reads an assignment by Id.
    /// </summary>
    /// <param name="id">The Id of the assignment to read.</param>
    /// <returns>The assignment with the specified Id, or null if not found.</returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(a => a.Id == id);
    }

    /// <summary>
    /// Reads all assignments, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all assignments.</param>
    /// <returns>An enumerable of assignments.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter == null ? DataSource.Assignments : DataSource.Assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    /// <exception cref="DalNotExistException">Thrown when the assignment with the specified Id does not exist.</exception>
    public void Update(Assignment item)
    {
        var existingAssignment = Read(item.Id);
        if (existingAssignment is null)
            throw new DalNotExistException($"Assignment with Id ={item.Id} doesn't exist");
        DataSource.Assignments.Remove(existingAssignment);
        DataSource.Assignments.Add(item);
    }

    /// <summary>
    /// Reads an assignment by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The assignment that matches the filter, or null if not found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
}
