namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Assignment with Id ={item.Id} already exists ");
        DataSource.Assignments.Add(item);
    }

    public void Delete(int id)
    {
        var assignment = Read(id);
        if (assignment is null)
            throw new DalNotExistException($"Assignment with Id ={id} doesn t exists");
        DataSource.Assignments.Remove(assignment);
    }
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(a => a.Id == id);
    }

   
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter == null ? DataSource.Assignments : DataSource.Assignments.Where(filter);
    }
    public void Update(Assignment item)
    {
        var existingAssignment = Read(item.Id);
        if (existingAssignment is null)
            throw new DalNotExistException($"Assignment with Id ={item.Id} doesn t exists");
        DataSource.Assignments.Remove(existingAssignment);
        DataSource.Assignments.Add(item);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        throw new NotImplementedException();
    }
}
