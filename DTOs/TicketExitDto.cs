using System.ComponentModel.DataAnnotations;

namespace CrudPark.API.DTOs;

public class TicketExitDto
{
    // Usaremos uno de estos dos campos para buscar el ticket activo
    public string? LicensePlate { get; set; }
    public string? Folio { get; set; } 

    // El ID del operador que registra la salida (Auditoría)
    [Required(ErrorMessage = "El ID del operador de salida es obligatorio.")]
    public int ExitOperatorId { get; set; }
    
    // El monto pagado por el cliente (Opcional, si quieres registrar el pago en el ticket)
    public decimal AmountPaid { get; set; } 
}