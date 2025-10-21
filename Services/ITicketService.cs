using CrudPark.API.Models;

namespace CrudPark.API.Services;

public interface ITicketService
{
    Task<Ticket> RegisterEntryAsync(Ticket newTicket);
    Task<Ticket> RegisterExitAsync(string identifier, int exitOperatorId);
    Task<Ticket?> GetTicketStatusAsync(string identifier);
}