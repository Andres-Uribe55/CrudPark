using CrudPark.API.Models;

namespace CrudPark.API.Repositories;

public interface ITicketRepository
{
    // CRUD Básico
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task<Ticket?> GetByIdAsync(int id);
    
    // Consultas Específicas
    Task<Ticket?> GetActiveTicketByLicensePlateAsync(string licensePlate);
    Task<Ticket?> GetActiveTicketByFolioAsync(string folio);
    Task<bool> HasActiveTicketAsync(string licensePlate);
}