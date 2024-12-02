namespace DO;

public record Volunteer
(
    int Id,
    string Name,
    string Phone,
    string Email,
    string? Password = null,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    RoleType Role = RoleType.Volunteer,
    bool IsActive = true,
    double? MaxDistance = null,
    DistanceType DistanceType = DistanceType.Area
)
{
    public Volunteer() : this(0, "", "", "") { } // for ch3
}

