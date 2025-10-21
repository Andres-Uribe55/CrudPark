
namespace CrudPark.API.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    // La dificultad por defecto (10) es segura para la mayoría de las aplicaciones.
    // Un costo mayor significa más seguridad, pero más tiempo de procesamiento.
    private const int HashWorkFactor = 10; 

    public string HashPassword(string password)
    {
        // BCrypt maneja automáticamente el 'salt' (valor aleatorio) y lo incluye en el hash final.
        return BCrypt.Net.BCrypt.HashPassword(password, HashWorkFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // BCrypt extrae el salt del hashedPassword y lo usa para verificar.
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Manejar un caso donde el hash no tiene el formato BCrypt esperado (ej. hash viejo o inválido)
            return false;
        }
    }
}