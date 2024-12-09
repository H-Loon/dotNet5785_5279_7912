namespace BlImplementation;

using Helpers;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>  
    /// Adds a new volunteer to the system.  
    /// </summary>  
    /// <param name="volunteer">The volunteer to be added.</param>  
    /// <exception cref="ArgumentException">Thrown when the volunteer is not valid or already exists.</exception>  
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            if (VolunteerManager.BOVolunteerCheck(volunteer) is false)
                throw new BO.BlNotValidEntityException("Volunteer is not valid");

            if (_dal.Volunteer.Read(volunteer.Id) is not null)
                throw new BO.BlAlreadyExistsException("Volunteer already exists");

            VolunteerManager.DOVolunteerFiller(volunteer);
            _dal.Volunteer.Create(VolunteerManager.ConvertToDO(volunteer));
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>  
    /// Deletes a volunteer from the system.  
    /// </summary>  
    /// <param name="id">The ID of the volunteer to be deleted.</param>  
    /// <exception cref="ArgumentException">Thrown when the volunteer is in treatment or has no completed calls.</exception>  
    public void DeleteVolunteer(int id)
    {
        try
        {
            var boVolunteer = VolunteerManager.ConvertToBO(id);

            if (boVolunteer.CurrentCall is not null)
                throw new BO.BlDeletionImpossibleException("Volunteer is in treatment");

            if (boVolunteer.CompletedCalls is 0)
                throw new BO.BlDeletionImpossibleException("Volunteer has no completed calls");

            _dal.Volunteer.Delete(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>  
    /// Retrieves a volunteer by ID.  
    /// </summary>  
    /// <param name="id">The ID of the volunteer to be retrieved.</param>  
    /// <returns>The volunteer with the specified ID.</returns>  
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

    /// <summary>  
    /// Retrieves a list of volunteers based on their active status and sorts them by the specified field.  
    /// </summary>  
    /// <param name="active">The active status to filter volunteers by. If null, all volunteers are returned.</param>  
    /// <param name="field">The field to sort the volunteers by.</param>  
    /// <returns>A list of volunteers matching the specified active status and sorted by the specified field.</returns>  
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

    /// <summary>  
    /// Logs in a volunteer using their name and password.  
    /// </summary>  
    /// <param name="name">The name of the volunteer.</param>  
    /// <param name="password">The password of the volunteer.</param>  
    /// <returns>The role of the volunteer if the login is successful.</returns>  
    /// <exception cref="ArgumentException">Thrown when the volunteer name is not found.</exception>  
    /// <exception cref="BO.BlIncorrectPasswordException">Thrown when the password is incorrect.</exception>  
    public BO.BoRoleType Login(string name, string? password)
    {
        try
        {
            DO.Volunteer? volunteer = _dal.Volunteer.Read(v => v.Name == name) ?? throw new BO.BlNotExistException("Volunteer name not found");

            if (volunteer.Password is null)
                return (BO.BoRoleType)volunteer.Role;

            if (VolunteerManager.CryptPW(password) != volunteer.Password)
                throw new BO.BlIncorrectPasswordException("Password is incorrect");

            else
                return (BO.BoRoleType)volunteer.Role;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>  
    /// Updates a volunteer's information.  
    /// </summary>  
    /// <param name="id">The ID of the volunteer to be updated.</param>  
    /// <param name="volunteer">The updated volunteer information.</param>  
    /// <exception cref="ArgumentException">Thrown when the volunteer is not valid, not found, or the user is not allowed to update the volunteer.</exception>  
    public void UpdateVolunteer(int id, BO.Volunteer volunteer)
    {
        try
        {
            var asker = _dal.Volunteer.Read(id) ?? throw new ArgumentException("Volunteer not found");

            if (VolunteerManager.BOVolunteerCheck(volunteer) is false)
                throw new BO.BlNotValidEntityException("Volunteer is not valid");

            if (asker.Role is not DO.RoleType.Admin)
            {
                if (asker.Id != volunteer.Id || volunteer.Role == BO.BoRoleType.Admin)
                    throw new BO.BlNotAllowedException("You are not allowed to update this volunteer");
            }

            VolunteerManager.DOVolunteerFiller(volunteer);
            _dal.Volunteer.Update(VolunteerManager.ConvertToDO(volunteer));
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}

