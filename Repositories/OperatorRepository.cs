using CrudPark.API.Data;
using CrudPark.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudPark.API.Repositories;

public class OperatorRepository : IOperatorRepository
{
    private readonly AppDbContext _context;

    public OperatorRepository(AppDbContext context)
    {
        _context = context;
    }

    // Implementación CRUD (Omitida por brevedad, sigue el patrón de Rate/Membership)
    public async Task<IEnumerable<Operator>> GetAllAsync() => await _context.Operators.ToListAsync();
    public async Task<Operator?> GetByIdAsync(int id) => await _context.Operators.FindAsync(id);

    // Método de creación
    public async Task<Operator> CreateAsync(Operator @operator)
    {
        // Asumiendo que el campo Password ya tiene el hash, se guarda directamente.
        @operator.CreatedAt = DateTimeOffset.UtcNow;
        @operator.IsActive = true; 
        _context.Operators.Add(@operator);
        await _context.SaveChangesAsync();
        return @operator;
    }

    // Método de actualización
    public async Task<Operator> UpdateAsync(Operator @operator)
    {
        // El Hashing de contraseña (si aplica) se maneja en el Servicio.
        @operator.UpdatedAt = DateTimeOffset.UtcNow;
        _context.Operators.Update(@operator);
        await _context.SaveChangesAsync();
        return @operator;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var @operator = await GetByIdAsync(id);
        if (@operator == null) return false;
        _context.Operators.Remove(@operator);
        await _context.SaveChangesAsync();
        return true;
    }

    // Lógica Específica

    public async Task<Operator?> GetByUsernameAsync(string username)
    {
        return await _context.Operators
                             .FirstOrDefaultAsync(o => o.Username == username);
    }
    
    // Verifica si el Username ya existe, excluyendo el ID actual (para la edición)
    public async Task<bool> ExistsUsernameAsync(string username, int excludeId = 0)
    {
        return await _context.Operators
                             .AnyAsync(o => o.Username == username && o.Id != excludeId);
    }
}