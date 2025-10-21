using CrudPark.API.Models;

namespace CrudPark.API.Services;

public interface IOperatorService
{
    // CRUD Básico
    Task<IEnumerable<Operator>> GetAllAsync();
    Task<Operator?> GetByIdAsync(int id);
    
    // Operaciones con Lógica de Negocio
    Task<Operator> CreateAsync(Operator @operator);
    Task<Operator> UpdateAsync(int id, Operator @operator);
    Task<bool> UpdatePasswordAsync(int id, string newPassword);
    Task<bool> DeleteAsync(int id);
    
    // Autenticación (Para el login)
    Task<Operator?> AuthenticateAsync(string username, string password);
}