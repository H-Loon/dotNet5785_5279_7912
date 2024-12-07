using BO;
using DalApi;
using DO;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static bool ValidateCall(BO.Call call)
{
    if (string.IsNullOrWhiteSpace(call.Address))
        throw new ArgumentException("Address cannot be null or empty.");

    if (call.Id <= 0)
        throw new ArgumentException("Id must be a positive integer.");

    if (call.Latitude < -90 || call.Latitude > 90)
        throw new ArgumentException("Latitude must be between -90 and 90.");

    if (call.Longitude < -180 || call.Longitude > 180)
        throw new ArgumentException("Longitude must be between -180 and 180.");

    if (call.StartTime == default)
        throw new ArgumentException("StartTime must be a valid date.");

    if (call.MaxTime.HasValue && call.MaxTime <= call.StartTime)
        throw new ArgumentException("MaxTime must be greater than StartTime.");

    if (!Enum.IsDefined(typeof(BoCallType), call.CallType))
        throw new ArgumentException("Invalid CallType.");

    if (!Enum.IsDefined(typeof(BoCallStatus), call.Status))
        throw new ArgumentException("Invalid CallStatus.");

    if (call.AssignInList != null)
    {
        foreach (var assign in call.AssignInList)
        {
            if (assign.VolunteerId <= 0)
                throw new ArgumentException("VolunteerId must be a positive integer.");

            if (string.IsNullOrWhiteSpace(assign.VolunteerName))
                throw new ArgumentException("VolunteerName cannot be null or empty.");

            if (assign.AssignTime == default)
                throw new ArgumentException("AssignTime must be a valid date.");

            if (assign.EndedTime.HasValue && assign.EndedTime <= assign.AssignTime)
                throw new ArgumentException("EndedTime must be greater than AssignTime.");

            if (assign.EndType.HasValue && !Enum.IsDefined(typeof(BoAssignmentEndReason), assign.EndType))
                throw new ArgumentException("Invalid EndType.");
        }
    }

    return true;
}
    internal static DO.Call converttoCall(BO.Call call)
    {
        var doCall = new DO.Call
        {

            Type = (DO.CallType)call.CallType,
            Address = call.Address,
            Latitude = call.Latitude.Value,
            Longitude = call.Longitude.Value,
            StartTime = call.StartTime,
            Description = call.Description,
            MaxTime = call.MaxTime,
        };

        return doCall;
    }
    internal static void CheckStatus(BO.Call call) 
    {
        if (call == null)
        {
            throw new KeyNotFoundException("Call not found.");
        }
        if (call.Status != BoCallStatus.Open || (call.AssignInList != null && call.AssignInList.Any()))
        {
            throw new InvalidOperationException("Cannot delete call. The call is either not open or has been assigned to a volunteer.");
        }
    }




}

//bool ValidateCall(BO.Call call)
//{
//    if (string.IsNullOrWhiteSpace(call.Address))
//        throw new ArgumentException("Address cannot be null or empty.");
//    if(call.Id)
//    if (call.Latitude < -90 || call.Latitude > 90)
//        throw new ArgumentException("Latitude must be between -90 and 90.");
//    if (call.Longitude < -180 || call.Longitude > 180)
//        throw new ArgumentException("Longitude must be between -180 and 180.");
//    if (call.StartTime == default)
//        throw new ArgumentException("StartTime must be a valid date.");
//    if (call.MaxTime.HasValue && call.MaxTime <= call.StartTime)
//        throw new ArgumentException("MaxTime must be greater than StartTime.");
//    return true;
//}



