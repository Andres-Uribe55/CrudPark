namespace CrudPark.API.Models;

public class Membership
{
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; } 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}