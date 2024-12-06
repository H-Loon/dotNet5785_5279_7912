using Helpers;

namespace BO;

public class Volunteer
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public BoRoleType Role { get; set; }
    public required bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public BoDistanceType DistanceType { get; set; }
    public int CompletedCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int OverDatedCalls { get; set; }
    public CallInProgress? CurrentCall { get; set; }
    public override string ToString() => this.ToStringProperty();
}
