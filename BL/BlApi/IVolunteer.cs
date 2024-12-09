namespace BlApi;

public interface IVolunteer
{
    BO.BoRoleType Login(string name, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteerInList(bool? active, BO.VolunteerInListField? field);
    BO.Volunteer GetVolunteer(int id);
    void UpdateVolunteer(int id, BO.Volunteer volunteer);
    void DeleteVolunteer(int id);
    void AddVolunteer(BO.Volunteer volunteer);
}
