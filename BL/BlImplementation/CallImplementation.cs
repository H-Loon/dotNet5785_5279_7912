//namespace BlImplementation;
//using BlApi;
//using System.Collections.Generic;
//using Helpers;

//internal class CallImplementation : ICall
//{
//    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
//    //private readonly List<BO.Call> _calls = new List<BO.Call>();//

//    public int[] GetCallsQuantities()//retourne un tableau dans lequel chaque index représente le nombre d'appels ayant un certain statut
//    {
//        var calls = _dal.Call.ReadAll();
//        var quantities =calls.GroupBy(c=> c.CallType).OrderBy(g => (int)g.Key).Select(g => g.Count()).ToArray();
//        return quantities;

//        //throw new NotImplementedException();


//        //return _calls
//        //    .GroupBy(call => (int)call.Status) // On groupe par statut de l'appel transformé en int
//        //    .OrderBy(g => g.Key) //  ordonne en fonction de la val de chq clé cree par groupby
//        //    .Select(g => g.Count()) //  compte le nombre d'appels pour chaque groupe
//        //    .ToArray(); //  retourne le résultat sous forme de tableau
//    }
//    public void AddCall(BO.Call call)
//    {

//        // Création de l'objet DO.Call à partir de l'objet BO.Call
//        CallManager.ValidateCall(call);

//        DO.Call dataCall = CallManager.ConvertToDoCall(call);

//        try
//        {
//            // Tentative d'ajout de la nouvelle appel à la couche de données
//            _dal.Call.Create(dataCall);
//        }
//        catch (Exception ex) 
//        {
//            // Capture de l'exception et relance d'une exception appropriée vers la couche de présentation
//            throw new InvalidOperationException("A call with the same ID already exists.", ex);
//        }
//    }

//    public void DeleteCall(int id)
//    {
//        try
//        {
//            var call = _dal.Call.Read(id);

//            CallManager.CheckStatus(call);
  
//            _dal.Call.Delete(id);
//        }
//        catch (KeyNotFoundException ex)
//        {
            
//            throw new InvalidOperationException("Call not found.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new InvalidOperationException("An error occurred while trying to delete the call.", ex);
//        }
//    }
//    public void UpdateCall(BO.Call call)
//    {

//        throw new NotImplementedException();
    
//    }






//    public BO.Call GetCall(int callid)
//    {
//        throw new NotImplementedException();
//    }

//    public IEnumerable<BO.CallInList> GetCallsInList(BO.CallInListField? field1, object? obj, BO.CallInListField? field2)
//    {
//        throw new NotImplementedException();
//    }

    

//    public IEnumerable<BO.ClosedCallInList> GetClosedCallByVolunteer(int id, BO.BoCallType? boCallType, BO.ClosedCallInListField? field)
//    {
//        throw new NotImplementedException();
//    }

//    public IEnumerable<BO.OpenCallInList> GetOpenCallForVolunteer(int id, BO.BoCallType? boCallType, BO.OpenCallInListField? field)
//    {
//        throw new NotImplementedException();
//    }

//    public void CompleteCall(int volunteerId, int assignmentId)
//    {
//        throw new NotImplementedException();
//    }

//    public void CancelCall(int cancelerId, int assignmentId)
//    {
//        throw new NotImplementedException();
//    }

//    public void AssignCall(int volunteerId, int callId)
//    {
//        throw new NotImplementedException();
//    }
//}


