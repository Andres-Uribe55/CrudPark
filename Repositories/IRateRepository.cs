using CrudPark.API.Models;

namespace CrudPark.API.Repositories;

public interface IRateRepository
{
    Task<IEnumerable<Rate>> GetAllRatesAsync();
    Task<Rate?> GetRateByIdAsync(int id);
    Task<Rate> CreateRateAsync(Rate rate);
    Task<Rate> UpdateRateAsync(Rate rate);
    Task DeleteRateAsync(int id);
    

    Task<Rate?> GetActiveRateByVehicleTypeAsync(VehicleType vehicleType, int excludeRateId = 0);
}