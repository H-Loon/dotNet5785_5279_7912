namespace BlImplementation;
using BlApi;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
       
    }

    public void DeleteVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer GetVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerInList(bool? active, BO.VolunteerInListField field)
    {
        throw new NotImplementedException();
    }

    public BO.BoRoleType LogIn(string name, string password)
    {
        try
        {
            DO.Volunteer? volunteer = _dal.Volunteer.Read(v => v.Name == name) ?? throw new ArgumentException("Volunteer not found");

            if (volunteer.Password != password)
                throw new ArgumentException("Password is incorrect");
            
            else
                return (BO.BoRoleType)volunteer.Role;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void UpdateVolunteer(int id, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}

