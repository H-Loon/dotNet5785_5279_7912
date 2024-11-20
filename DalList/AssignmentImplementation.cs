namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        DataSource.Assignments.Add(item with { Id = Config.NextAssignmementId });
    }

    public void Delete(int id)
    {
        if (Read(id) is Assignment assignment)
            DataSource.Assignments.Remove(assignment);
        else
            throw new Exception($"Assignment with Id={id} does not exist");
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(a => a.Id == id);
    }

    //public List<Assignment> ReadAll()
    //{
    //    return new List<Assignment>(DataSource.Assignments);
    //}
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter == null ? DataSource.Assignments : DataSource.Assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        if (Read(item.Id) is Assignment assignment)
        {
            DataSource.Assignments.Remove(assignment);
            DataSource.Assignments.Add(item);
        }
        else
            throw new Exception($"Assignment with Id={item.Id} does not exist");
    }
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
