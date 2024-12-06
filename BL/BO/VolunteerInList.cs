using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public bool Active { get; set; }
    public int CompletedCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int? CallInTreatment { get; set; }
    public BoCallType CurrentCallType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
