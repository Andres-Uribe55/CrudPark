namespace CrudPark.API.Services;

public interface IPasswordHasher
{
    // Genera un hash seguro a partir de una contraseña de texto plano
    string HashPassword(string password);

    // Verifica una contraseña de texto plano contra un hash almacenado
    bool VerifyPassword(string password, string hashedPassword);
}