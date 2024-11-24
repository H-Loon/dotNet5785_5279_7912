namespace Dal;
using DalApi;
using DO;

internal class VolunteerImplementation : IVolunteer
{

    

    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with Id ={item.Id} already exists ");
        DataSource.Volunteers.Add(item);
    }

    
    public void Delete(int id)
    {
        Volunteer volunteer = Read(id) ?? throw new DalNotExistException($"Assignment with Id ={id} doesn t exists");
        DataSource.Volunteers.Remove(volunteer);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null ? DataSource.Volunteers : DataSource.Volunteers.Where(filter);
    }




    public void Update(Volunteer item)
    {
        var existingVolunteer = Read(item.Id) ?? throw new DalNotExistException($"Volunteer with Id ={item.Id} doesn t exists");
        DataSource.Volunteers.Remove(existingVolunteer);
        DataSource.Volunteers.Add(item);
    }


    public Volunteer? Read (Func<Volunteer,bool>filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
