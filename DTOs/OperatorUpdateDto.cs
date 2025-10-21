using System.ComponentModel.DataAnnotations;

namespace CrudPark.API.DTOs;

public class OperatorUpdateDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(30, MinimumLength = 5)]
    public string Username { get; set; } = string.Empty;
    
    // Campo administrativo para activar/inactivar
    [Required(ErrorMessage = "El estado de actividad es obligatorio.")]
    public bool IsActive { get; set; }
}