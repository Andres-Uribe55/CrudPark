using CrudPark.API.Models;

namespace CrudPark.API.Repositories;

public interface IOperatorRepository
{
    // CRUD Básico
    Task<IEnumerable<Operator>> GetAllAsync();
    Task<Operator?> GetByIdAsync(int id);
    Task<Operator> CreateAsync(Operator @operator);
    Task<Operator> UpdateAsync(Operator @operator);
    Task<bool> DeleteAsync(int id);

    // Lógica Específica para la DB
    Task<Operator?> GetByUsernameAsync(string username);
    Task<bool> ExistsUsernameAsync(string username, int excludeId = 0);
}