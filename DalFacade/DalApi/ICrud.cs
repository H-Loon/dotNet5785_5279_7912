using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi;


/// <summary>
/// Generic interface for CRUD operations.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface ICrud<T> where T : class
{
    /// <summary>
    /// Creates a new entity object in the DAL.
    /// </summary>
    /// <param name="item">The entity object to create.</param>
    void Create(T item);

    /// <summary>
    /// Reads an entity object by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to read.</param>
    /// <returns>The entity object with the specified ID, or null if not found.</returns>
    T? Read(int id);

    /// <summary>
    /// Reads all entity objects, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply, or null to return all entities.</param>
    /// <returns>An enumerable of entity objects.</returns>
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null);

    /// <summary>
    /// Updates an existing entity object.
    /// </summary>
    /// <param name="item">The entity object to update.</param>
    void Update(T item);

    /// <summary>
    /// Deletes an entity object by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    void Delete(int id);

    /// <summary>
    /// Deletes all entity objects.
    /// </summary>
    void DeleteAll();

    /// <summary>
    /// Reads an entity object by a filter predicate.
    /// </summary>
    /// <param name="filter">The filter predicate to apply.</param>
    /// <returns>The entity object that matches the filter, or null if not found.</returns>
    T? Read(Func<T, bool> filter);
}
