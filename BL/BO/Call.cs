using Helpers;

namespace BO;

public class Call
{
    public int Id { get; init; }
    public BoCallType CallType { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? MaxTime { get; set; }
    public BoCallStatus Status { get; set; }
    public List<BO.CallAssignInList>? AssignInList { get; set; }
    public override string ToString() => this.ToStringProperty();
}
