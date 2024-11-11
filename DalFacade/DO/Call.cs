namespace DO;
/// <summary>
/// Create a call
/// </summary>
/// <param name="Id">Call Id</param>
/// <param name="Type">Type of the call</param>
/// <param name="Address">Full adress of the caller</param>
/// <param name="Latitude">Latidtude coordonate</param>
/// <param name="Longitude">Longitute coordonate</param>
/// <param name="StartTime">Time when the call has been open</param>
/// <param name="Description">Description of the call</param>
/// <param name="MaxTime">The max time given to close the call</param>
public record Call
(
    int Id,
    CallType Type,
    string Address,
    double Latitude,
    double Longitude,
    DateTime StartTime,
    string? Description = null,
    DateTime? MaxTime = null
)
{
    public Call() : this(0, CallType.Other, "", 0, 0, DateTime.Now) { } // for ch3
}