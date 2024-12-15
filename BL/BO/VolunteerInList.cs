using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public bool IsActive { get; init; }
    public int CompletedCalls { get; init; }
    public int CanceledCalls { get; init; }
    public int? CallInTreatment { get; init; }
    public BoCallType CurrentCallType { get; init; }
    public override string ToString() => this.ToStringProperty();
}
