namespace BlImplementation;
using BlApi;
using DalApi;
using Helpers;
using System.Reflection;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try 
        {
            if (VolunteerManager.BOVolunteerCheck(volunteer) is false)
                throw new ArgumentException("Volunteer is not valid");

            if (_dal.Volunteer.Read(volunteer.Id) is not null)
                throw new ArgumentException("Volunteer already exists");

            VolunteerManager.CreateDOVolunteer(volunteer);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            var boVolunteer = VolunteerManager.ConvertToBO(id);

            if (boVolunteer.CurrentCall is not null)
                throw new ArgumentException("Volunteer is in treatment");
          
            if (boVolunteer.CompletedCalls is 0)
                throw new ArgumentException("Volunteer has no completed calls");

            _dal.Volunteer.Delete(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public BO.Volunteer GetVolunteer(int id)
    {
        try
        {
            return VolunteerManager.ConvertToBO(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerInList(bool? active, BO.VolunteerInListField? field)
    {
        try 
        { 
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
            IEnumerable<BO.VolunteerInList> volunteers = VolunteerManager.GetVolunteerInLists(active);

            return field switch
            {
                BO.VolunteerInListField.Name => volunteers.OrderBy(v => v.Name),
                BO.VolunteerInListField.Active => volunteers.OrderBy(v => v.IsActive),
                BO.VolunteerInListField.CompletedCalls => volunteers.OrderBy(v => v.CompletedCalls),
                BO.VolunteerInListField.CanceledCalls => volunteers.OrderBy(v => v.CanceledCalls),
                BO.VolunteerInListField.CallInTreatment => volunteers.OrderBy(v => v.CallInTreatment),
                BO.VolunteerInListField.CurrentCallType => volunteers.OrderBy(v => v.CurrentCallType),
                _ => volunteers.OrderBy(v => v.Id),
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
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
        try
        {
            var asker = _dal.Volunteer.Read(id) ?? throw new ArgumentException("Volunteer not found");

            if (VolunteerManager.BOVolunteerCheck(volunteer) is false)
                throw new ArgumentException("Volunteer is not valid");

            if (asker.Role is not DO.RoleType.Admin)
            {
                if (asker.Id != volunteer.Id || volunteer.Role == BO.BoRoleType.Admin)
                    throw new ArgumentException("You are not allowed to update this volunteer");
            }

            VolunteerManager.CreateDOVolunteer(volunteer);

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}

