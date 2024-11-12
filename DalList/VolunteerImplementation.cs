namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new Exception($"Volunteer with Id={item.Id} already exist");
        else
            DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        if (Read(id) is Volunteer volunteer)
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
        return DataSource.Volunteers.Find(v => v.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        if (Read(item.Id) is Volunteer volunteer)
        {
            DataSource.Volunteers.Remove(volunteer);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new Exception($"Volunteer with Id={item.Id} does not exist");
    }
}
