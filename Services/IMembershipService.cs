using CrudPark.API.Models;

namespace CrudPark.API.Services;

public interface IMembershipService
{
    Task<IEnumerable<Membership>> GetAllMembershipsAsync();
    Task<Membership?> GetMembershipByIdAsync(int id);
    Task<IEnumerable<Membership>> SearchByLicensePlateAsync(string licensePlate);
    Task<Membership> CreateMembershipAsync(Membership membership);
    Task<Membership> UpdateMembershipAsync(int id, Membership membership);
    Task<bool> ToggleMembershipStatusAsync(int id);
    Task<IEnumerable<Membership>> GetExpiringMembershipsAsync(int daysUntilExpiration);
}