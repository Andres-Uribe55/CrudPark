using System.ComponentModel.DataAnnotations;

namespace CrudPark.API.DTOs;

public class MembershipCreateDto
{
    // Cliente
    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(60)]
    public string ClientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [StringLength(150)]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    // Vehículo
    [Required(ErrorMessage = "La placa es obligatoria.")]
    [StringLength(20)]
    [RegularExpression(@"[a-zA-Z0-9]{4,10}$", ErrorMessage = "La placa debe contener 6 caracteres alfanuméricos.")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de vehículo es obligatorio.")]
    [Range(1, 4, ErrorMessage = "El tipo de vehículo debe ser un valor entre 1 y 4.")] 
    public int VehicleType { get; set; } 

    // Fechas
    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime EndDate { get; set; }
}