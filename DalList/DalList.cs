using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;
using DO;

sealed public class DalList : IDal
{
    public ICall Call { get; } = new CallImplementation();

    public IAssignment Assignment = new AssignmentImplementation();
    public IConfig Config { get; }= new ConfigImplementation();

    IVolunteer IDal.Volunteer => throw new NotImplementedException();//ca la mm automatic qd erreur corrigé

    IAssignment IDal.Assignment => throw new NotImplementedException();//ca la mm automatic qd erreur corrigé

    public IVolunteer Volunteer = new VolunteerImplementation();

    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}