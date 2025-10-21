using CrudPark.API.Models;
using CrudPark.API.Repositories;
using System.Collections.Generic;

namespace CrudPark.API.Services;

public class OperatorService : IOperatorService
{
    private readonly IOperatorRepository _operatorRepository;
    private readonly IPasswordHasher _passwordHasher; // Inyección

    public OperatorService(IOperatorRepository operatorRepository, IPasswordHasher passwordHasher)
    {
        _operatorRepository = operatorRepository;
        _passwordHasher = passwordHasher;
    }

    // ... (Métodos GetAll, GetById, Delete omitidos por brevedad) ...
    public async Task<IEnumerable<Operator>> GetAllAsync() => await _operatorRepository.GetAllAsync();
    public async Task<Operator?> GetByIdAsync(int id) => await _operatorRepository.GetByIdAsync(id);
    public async Task<bool> DeleteAsync(int id) => await _operatorRepository.DeleteAsync(id);

    // --- Lógica de Creación (Hashing y Unicidad) ---

    public async Task<Operator> CreateAsync(Operator @operator)
    {
        // 1. Verificar unicidad de Username
        if (await _operatorRepository.ExistsUsernameAsync(@operator.Username))
        {
            throw new InvalidOperationException($"El nombre de usuario '{@operator.Username}' ya está en uso.");
        }
        
        // 2. HASHING DE CONTRASEÑA: La clave de la seguridad
        // El operador tiene la contraseña en texto plano (del DTO), la hasheamos.
        @operator.Password = _passwordHasher.HashPassword(@operator.Password);

        // 3. Crear en el repositorio
        return await _operatorRepository.CreateAsync(@operator);
    }

    // --- Lógica de Actualización (Datos Personales y Unicidad) ---

    public async Task<Operator> UpdateAsync(int id, Operator @operator)
    {
        var existing = await _operatorRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Operador con ID {id} no encontrado.");
        
        // 1. Verificar unicidad de Username, excluyendo el ID actual
        if (await _operatorRepository.ExistsUsernameAsync(@operator.Username, id))
        {
            throw new InvalidOperationException($"El nombre de usuario '{@operator.Username}' ya está en uso por otro operador.");
        }
        
        // 2. Transferir el HASH de la contraseña existente
        @operator.Password = existing.Password;

        // 3. Transferir campos de auditoría y ID
        @operator.Id = id;
        @operator.CreatedAt = existing.CreatedAt;

        // 4. Actualizar en el repositorio
        return await _operatorRepository.UpdateAsync(@operator);
    }
    
    // --- Actualización de Contraseña (Solo Hashing) ---
    
    public async Task<bool> UpdatePasswordAsync(int id, string newPassword)
    {
        var existing = await _operatorRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        // 1. HASHING DE LA NUEVA CONTRASEÑA
        existing.Password = _passwordHasher.HashPassword(newPassword);

        // 2. Actualizar el operador (solo el hash cambia en la DB)
        await _operatorRepository.UpdateAsync(existing);
        return true;
    }

    // --- Autenticación (Para la aplicación Java) ---

    public async Task<Operator?> AuthenticateAsync(string username, string password)
    {
        var @operator = await _operatorRepository.GetByUsernameAsync(username);

        // 1. Verificar si el operador existe y si está activo
        if (@operator == null || !@operator.IsActive)
            return null; // Usuario no encontrado o inactivo
        
        // 2. Verificar la contraseña usando el hash
        bool isPasswordValid = _passwordHasher.VerifyPassword(password, @operator.Password);

        return isPasswordValid ? @operator : null;
    }
}