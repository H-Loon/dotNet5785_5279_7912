using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;
sealed public class DalList : IDal
{
    public ICall Call => throw new NotImplementedException();

    public IVolunteer Volunteer => throw new NotImplementedException();

    public IAssignment Assignment => throw new NotImplementedException();

    public void ResetDB()
    {
        throw new NotImplementedException();
    }
}