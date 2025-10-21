using CrudPark.API.Models;

namespace CrudPark.API.Services;

public interface IRateService
{
    Task<IEnumerable<Rate>> GetAllRatesAsync();
    Task<Rate?> GetRateByIdAsync(int id);
    
    Task<Rate> CreateRateAsync(Rate rate);
    Task<Rate> UpdateRateAsync(int id, Rate rate);
    Task<bool> DeleteRateAsync(int id);
}