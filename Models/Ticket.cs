namespace CrudPark.API.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public DateTimeOffset EntryDateTime { get; set; }
    public DateTimeOffset? ExitDateTime { get; set; }
    public EntryType EntryType { get; set; }
    public int EntryOperatorId { get; set; }
    public int? ExitOperatorId { get; set; }
    public int? TotalMinutes { get; set; }
    public int? MembershipId { get; set; }
    public string QRCode { get; set; } = string.Empty;
    public decimal? RateApplied { get; set; } 
    public decimal? TotalCost { get; set; }
    
    // Navigation Properties
    public Operator? EntryOperator { get; set; }
    public Operator? ExitOperator { get; set; }
    public Membership? Membership { get; set; }
}

public enum EntryType
{
    Membership,
    Guest
}