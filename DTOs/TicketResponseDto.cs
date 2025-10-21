using CrudPark.API.Models;

namespace CrudPark.API.DTOs;

public class TicketResponseDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty; // Como String
    
    public DateTimeOffset EntryDateTime { get; set; }
    public DateTimeOffset? ExitDateTime { get; set; }
    
    public string EntryType { get; set; } = string.Empty; // Como String
    
    public int EntryOperatorId { get; set; }
    public int? ExitOperatorId { get; set; }
    
    public int? TotalMinutes { get; set; }
    public decimal? TotalCost { get; set; }
    public decimal? RateApplied { get; set; }
    
    public string QRCode { get; set; } = string.Empty;
}