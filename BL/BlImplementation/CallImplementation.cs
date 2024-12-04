namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Threading.Tasks.Dataflow;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //private readonly List<BO.Call> _calls = new List<BO.Call>();//

    public int[] GetCallsQuantities()//retourne un tableau dans lequel chaque index représente le nombre d'appels ayant un certain statut
    {
        var calls = _dal.Call.ReadAll();
        var quantities =calls.GroupBy(c=> c.BoCallStatus).OrderBy(g => (int)g.Key).Select(g => g.Count()).ToArray();
        return quantities;

        //return _calls
        //    .GroupBy(call => (int)call.Status) // On groupe par statut de l'appel transformé en int
        //    .OrderBy(g => g.Key) //  ordonne en fonction de la val de chq clé cree par groupby
        //    .Select(g => g.Count()) //  compte le nombre d'appels pour chaque groupe
        //    .ToArray(); //  retourne le résultat sous forme de tableau
    }
    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2)
    {

    }


}


