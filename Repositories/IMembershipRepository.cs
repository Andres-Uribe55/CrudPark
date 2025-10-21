using CrudPark.API.Models;

namespace CrudPark.API.Repositories;

public interface IMembershipRepository
{
    Task<IEnumerable<Membership>> GetAllAsync();
    Task<Membership?> GetByIdAsync(int id);
    Task<Membership?> GetByLicensePlateAsync(string licensePlate);
    Task<Membership> CreateAsync(Membership membership);
    Task<Membership> UpdateAsync(Membership membership);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsActiveMembershipAsync(string licensePlate);
}