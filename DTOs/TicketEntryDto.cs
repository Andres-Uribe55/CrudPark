using System.ComponentModel.DataAnnotations;
using CrudPark.API.Models;

namespace CrudPark.API.DTOs;

public class TicketEntryDto
{
    [Required(ErrorMessage = "La placa es obligatoria.")]
    [StringLength(10, ErrorMessage = "Placa demasiado larga.")]
    public string LicensePlate { get; set; } = string.Empty;

    // Asumo que tu enum VehicleType está en CrudPark.API.Models
    [Required(ErrorMessage = "El tipo de vehículo es obligatorio.")]
    public VehicleType VehicleType { get; set; }

    // El ID del operador que registra la entrada (Auditoría)
    [Required(ErrorMessage = "El ID del operador de entrada es obligatorio.")]
    public int EntryOperatorId { get; set; }
}