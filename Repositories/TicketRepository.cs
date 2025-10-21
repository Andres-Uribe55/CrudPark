using CrudPark.API.Data;
using CrudPark.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudPark.API.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        // La mayor parte de la inicialización se hace en el Servicio
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        // Se asume que el objeto ticket ya tiene todos los cálculos hechos
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets.FindAsync(id);
    }
    
    // Busca un ticket que no tenga ExitDateTime (es decir, está activo)
    public async Task<Ticket?> GetActiveTicketByLicensePlateAsync(string licensePlate)
    {
        return await _context.Tickets
                             .Include(t => t.Membership) // Incluye la membresía si existe
                             .FirstOrDefaultAsync(t => t.LicensePlate == licensePlate && 
                                                       t.ExitDateTime == null);
    }
    
    // Busca un ticket activo por Folio
    public async Task<Ticket?> GetActiveTicketByFolioAsync(string folio)
    {
        return await _context.Tickets
                             .Include(t => t.Membership)
                             .FirstOrDefaultAsync(t => t.Folio == folio && 
                                                       t.ExitDateTime == null);
    }

    // Verifica si la placa ya tiene un ticket activo
    public async Task<bool> HasActiveTicketAsync(string licensePlate)
    {
        return await _context.Tickets
                             .AnyAsync(t => t.LicensePlate == licensePlate && 
                                            t.ExitDateTime == null);
    }
}