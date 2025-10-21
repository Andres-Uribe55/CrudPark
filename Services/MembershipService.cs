using CrudPark.API.Models;
using CrudPark.API.Repositories;

namespace CrudPark.API.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;

    public MembershipService(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
    {
        return await _membershipRepository.GetAllAsync();
    }

    public async Task<Membership?> GetMembershipByIdAsync(int id)
    {
        return await _membershipRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Membership>> SearchByLicensePlateAsync(string licensePlate)
    {
        var membership = await _membershipRepository.GetByLicensePlateAsync(licensePlate);
        return membership != null ? new[] { membership } : Array.Empty<Membership>();
    }

    public async Task<Membership> CreateMembershipAsync(Membership membership)
    {
        // Validación 1: Verificar que las fechas sean coherentes
        if (membership.EndDate <= membership.StartDate)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        // Validación 2: Verificar que no exista una mensualidad activa para esa placa
        var existsActive = await _membershipRepository.ExistsActiveMembershipAsync(membership.LicensePlate);
        if (existsActive)
        {
            throw new InvalidOperationException($"Ya existe una mensualidad activa para la placa {membership.LicensePlate}.");
        }

        // Establecer campos automáticos
        membership.CreatedAt = DateTime.Now;
        membership.IsActive = true;

        // Guardar en la base de datos
        return await _membershipRepository.CreateAsync(membership);
    }

    public async Task<Membership> UpdateMembershipAsync(int id, Membership membership)
    {
        // Verificar que existe
        var existing = await _membershipRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"No se encontró la mensualidad con ID {id}.");
        }

        // Validar fechas
        if (membership.EndDate <= membership.StartDate)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        // Actualizar campos
        existing.ClientName = membership.ClientName;
        existing.Email = membership.Email;
        existing.Phone = membership.Phone;
        existing.LicensePlate = membership.LicensePlate;
        existing.VehicleType = membership.VehicleType;
        existing.StartDate = membership.StartDate;
        existing.EndDate = membership.EndDate;
        existing.IsActive = membership.IsActive;
        existing.UpdatedAt = DateTime.Now;

        return await _membershipRepository.UpdateAsync(existing);
    }

    public async Task<bool> ToggleMembershipStatusAsync(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership == null)
        {
            throw new KeyNotFoundException($"No se encontró la mensualidad con ID {id}.");
        }

        // Cambiar el estado
        membership.IsActive = !membership.IsActive;
        membership.UpdatedAt = DateTime.Now;

        await _membershipRepository.UpdateAsync(membership);
        return membership.IsActive;
    }

    public async Task<IEnumerable<Membership>> GetExpiringMembershipsAsync(int daysUntilExpiration)
    {
        var allMemberships = await _membershipRepository.GetAllAsync();
        
        var expiringDate = DateTime.Now.AddDays(daysUntilExpiration);
        
        return allMemberships.Where(m => 
            m.IsActive && 
            m.EndDate <= expiringDate && 
            m.EndDate >= DateTime.Now
        ).ToList();
    }
}