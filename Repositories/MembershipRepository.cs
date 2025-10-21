using CrudPark.API.Data;
using CrudPark.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudPark.API.Repositories;

public class MembershipRepository : IMembershipRepository
{
    private readonly AppDbContext _context;

    public MembershipRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Membership>> GetAllAsync()
    {
        return await _context.Memberships
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Membership?> GetByIdAsync(int id)
    {
        return await _context.Memberships
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Membership?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _context.Memberships
            .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate);
    }

    public async Task<Membership> CreateAsync(Membership membership)
    {
        membership.CreatedAt = DateTimeOffset.UtcNow;
        membership.IsActive = true;
        
        _context.Memberships.Add(membership);
        await _context.SaveChangesAsync();
        return membership;
    }

    public async Task<Membership> UpdateAsync(Membership membership)
    {
        membership.UpdatedAt = DateTimeOffset.UtcNow;
        
        _context.Memberships.Update(membership);
        await _context.SaveChangesAsync();
        return membership;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var membership = await GetByIdAsync(id);
        if (membership == null)
            return false;

        _context.Memberships.Remove(membership);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsActiveMembershipAsync(string licensePlate)
    {
        return await _context.Memberships
            .AnyAsync(m => m.LicensePlate == licensePlate && 
                           m.IsActive);
    }
}