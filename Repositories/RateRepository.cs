using CrudPark.API.Data;
using CrudPark.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudPark.API.Repositories;

public class RateRepository : IRateRepository
{
    private readonly AppDbContext _context;

    public RateRepository(AppDbContext context)
    {
        _context = context;
    }
    

    public async Task<IEnumerable<Rate>> GetAllRatesAsync()
    {
        return await _context.Rates
                             .OrderBy(r => r.VehicleType)
                             .ToListAsync();
    }

    public async Task<Rate?> GetRateByIdAsync(int id)
    {
        return await _context.Rates.FindAsync(id);
    }

    public async Task<Rate> CreateRateAsync(Rate rate)
    {
        rate.CreatedAt = DateTimeOffset.UtcNow;
        rate.IsActive = true; 
        _context.Rates.Add(rate);
        await _context.SaveChangesAsync();
        return rate;
    }

    public async Task<Rate> UpdateRateAsync(Rate rate)
    {
        rate.UpdatedAt = DateTimeOffset.UtcNow;
        _context.Rates.Update(rate);
        await _context.SaveChangesAsync();
        return rate;
    }
    
    public async Task DeleteRateAsync(int id)
    {
        var rate = await _context.Rates.FindAsync(id);
        if (rate != null)
        {
            _context.Rates.Remove(rate);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<Rate?> GetActiveRateByVehicleTypeAsync(VehicleType vehicleType, int excludeRateId = 0)
    {
    
        return await _context.Rates
                             .Where(r => r.VehicleType == vehicleType && 
                                         r.IsActive && 
                                         r.Id != excludeRateId)
                             .FirstOrDefaultAsync();
    }
}