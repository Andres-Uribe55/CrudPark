using System.ComponentModel.DataAnnotations;

namespace CrudPark.API.DTOs;

public class RateUpdateDto
{
    // Nombre de la Tarifa
    [Required(ErrorMessage = "El nombre de la tarifa es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
    public string RateName { get; set; } = string.Empty;

    // Tipo de Vehículo
    [Required(ErrorMessage = "El tipo de vehículo es obligatorio.")]
    [Range(1, 4, ErrorMessage = "El tipo de vehículo debe ser un valor válido (1-4).")] // Asumimos 4 tipos
    public int VehicleType { get; set; }

    // Valor por Hora
    [Required(ErrorMessage = "La tarifa por hora es obligatoria.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El valor debe ser positivo.")]
    public decimal HourlyRate { get; set; }

    // Valor por Fracción
    [Required(ErrorMessage = "La tarifa fraccional es obligatoria.")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "El valor debe ser positivo o cero.")]
    public decimal FractionRate { get; set; }

    // Tope Diario
    [Required(ErrorMessage = "El tope diario es obligatorio.")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "El valor debe ser positivo o cero.")]
    public decimal DailyCap { get; set; }

    // Tiempo de Gracia
    [Required(ErrorMessage = "El tiempo de gracia es obligatorio.")]
    [Range(0, 60, ErrorMessage = "El tiempo de gracia debe estar entre 0 y 60 minutos.")]
    public int GracePeriodMinutes { get; set; }
    
}