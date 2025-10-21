using System.ComponentModel.DataAnnotations;

namespace CrudPark.API.DTOs;

public class OperatorPasswordUpdateDto
{
    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;

    // Campo de seguridad para validar la entrada
    [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
    [Compare(nameof(NewPassword), ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}