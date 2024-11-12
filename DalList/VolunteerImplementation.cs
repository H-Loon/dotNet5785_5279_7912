namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (DataSource.Volunteers.Find(v => v.Id == item.Id) is Volunteer)
            throw new Exception($"Volunteer with Id={item.Id} already exist");
        else
            DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        if (DataSource.Volunteers.Find(a => a.Id == id) is Volunteer volunteer)
            DataSource.Volunteers.Remove(volunteer);
        else
            throw new Exception($"Volunteer with Id={id} does not exist");
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(a => a.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        List<Volunteer> NewVolunteers = new();
        foreach (var volunteer in DataSource.Volunteers)
        {
            NewVolunteers.Add(volunteer with { });
        }
        return NewVolunteers;
    }

    public void Update(Volunteer item)
    {
        if (DataSource.Volunteers.Find(a => a.Id == item.Id) is Volunteer volunteer)
        {
            DataSource.Volunteers.Remove(volunteer);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new Exception($"Volunteer with Id={item.Id} does not exist");
    }
}
