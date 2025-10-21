using System.ComponentModel.DataAnnotations;
using CrudPark.API.Models; 

namespace CrudPark.API.DTOs;

public class MembershipUpdateDto
{
    [Required]
    [StringLength(60)]
    public string ClientName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;
    
    [Required] 
    [StringLength(20)] 
    [RegularExpression(@"^[a-zA-Z0-9]{4,10}$")] 
    public string LicensePlate { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 3)]
    public int VehicleType { get; set; } 
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; }
}