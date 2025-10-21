namespace CrudPark.API.Models;

public class Rate
{
    public int Id { get; set; }
    public string RateName { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal FractionRate { get; set; }
    public decimal DailyCap { get; set; }
    public int GracePeriodMinutes { get; set; } = 30;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}