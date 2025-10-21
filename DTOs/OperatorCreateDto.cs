using System.ComponentModel.DataAnnotations;

namespace CrudPark.API.DTOs;

public class OperatorCreateDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "El usuario debe tener entre 5 y 30 caracteres.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    // Campo de seguridad para validar la entrada
    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
    [Compare(nameof(Password), ErrorMessage = "La contraseña y la confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}