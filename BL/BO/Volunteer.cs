using Helpers;

namespace BO;

public class Volunteer
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
    public string? Password { get; set; }
    public string? Address { get; init; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public BoRoleType Role { get; init; }
    public required bool IsActive { get; init; }
    public double? MaxDistance { get; init; }
    public BoDistanceType DistanceType { get; init; }
    public int CompletedCalls { get; init; }
    public int CanceledCalls { get; init; }
    public int OverDatedCalls { get; init; }
    public CallInProgress? CurrentCall { get; init; }
    public override string ToString() => this.ToStringProperty();
}
