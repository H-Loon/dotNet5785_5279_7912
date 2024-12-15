using Helpers;

namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public BoCallType CallType { get; init; }
    public string? Description { get; init; }
    public required string Address { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? MaxTime { get; init; }
    public double CallDistance { get; init; }
    public override string ToString() => this.ToStringProperty();
}
