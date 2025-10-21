using CrudPark.API.Models;
using CrudPark.API.Repositories;

namespace CrudPark.API.Services;

public class RateService : IRateService
{
    private readonly IRateRepository _rateRepository;

    public RateService(IRateRepository rateRepository)
    {
        _rateRepository = rateRepository;
    }

    // --- Implementación CRUD Básico (Delegación al Repositorio) ---

    public async Task<IEnumerable<Rate>> GetAllRatesAsync()
    {
        return await _rateRepository.GetAllRatesAsync();
    }

    public async Task<Rate?> GetRateByIdAsync(int id)
    {
        return await _rateRepository.GetRateByIdAsync(id);
    }

    // --- Lógica de Negocio en Create ---

    public async Task<Rate> CreateRateAsync(Rate rate)
    {
        // 1. Aplicar lógica de unicidad si la tarifa se crea como activa.
        if (rate.IsActive)
        {
            var existingRate = await _rateRepository.GetActiveRateByVehicleTypeAsync(rate.VehicleType);
            
            if (existingRate != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe una tarifa activa para el tipo de vehículo '{rate.VehicleType}'. " +
                    "Debe inactivar la tarifa existente antes de activar una nueva."
                );
            }
        }
        
        // 2. Crear la tarifa
        return await _rateRepository.CreateRateAsync(rate);
    }

    // --- Lógica de Negocio en Update ---

    public async Task<Rate> UpdateRateAsync(int id, Rate rate)
    {
        var existing = await _rateRepository.GetRateByIdAsync(id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"No se encontró la tarifa con ID {id}.");
        }
        
        // 1. Aplicar lógica de unicidad si la tarifa se establece como activa.
        if (rate.IsActive)
        {
            // Buscamos otra tarifa activa, excluyendo la tarifa que estamos actualizando.
            var competingRate = await _rateRepository.GetActiveRateByVehicleTypeAsync(rate.VehicleType, id);

            if (competingRate != null)
            {
                throw new InvalidOperationException(
                    $"No se puede activar esta tarifa. Ya existe otra tarifa activa (ID: {competingRate.Id}) " +
                    $"para el tipo de vehículo '{rate.VehicleType}'."
                );
            }
        }
        
        // 2. Transferir datos de auditoría y ID para la actualización
        rate.Id = id;
        rate.CreatedAt = existing.CreatedAt;
        
        // 3. Actualizar la tarifa
        return await _rateRepository.UpdateRateAsync(rate);
    }
    
    // --- Implementación Delete ---

    public async Task<bool> DeleteRateAsync(int id)
    {
        var rate = await _rateRepository.GetRateByIdAsync(id);
        if (rate == null)
        {
            return false;
        }

        // Se podría agregar lógica de negocio aquí, ej:
        // if (rate.IsActive) throw new InvalidOperationException("No se puede eliminar una tarifa activa.");

        await _rateRepository.DeleteRateAsync(id);
        return true;
    }
}