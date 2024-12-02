namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public BoCallType CallType { get; set; }
    public string? Description { get; set; }
    public required string Address { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? MaxTime { get; set; }
    public double CallDistance { get; set; }
}
