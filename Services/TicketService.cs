using CrudPark.API.Models;
using CrudPark.API.Repositories;
using System.Threading.Tasks;

namespace CrudPark.API.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMembershipRepository _membershipRepository; // Para chequear membresías
    private readonly IRateRepository _rateRepository; // Para obtener la tarifa
    
    // Constante para el Folio (solo para este ejemplo, en producción usarías un generador)
    private static int _folioCounter = 1000; 

    public TicketService(ITicketRepository ticketRepository, IMembershipRepository membershipRepository, IRateRepository rateRepository)
    {
        _ticketRepository = ticketRepository;
        _membershipRepository = membershipRepository;
        _rateRepository = rateRepository;
    }
    
    // MÉTODO 1: REGISTRAR ENTRADA (Creación del Ticket)

    public async Task<Ticket> RegisterEntryAsync(Ticket newTicket)
    {
        // 1. Verificar si ya tiene un ticket activo
        if (await _ticketRepository.HasActiveTicketAsync(newTicket.LicensePlate))
        {
            throw new InvalidOperationException($"La placa {newTicket.LicensePlate} ya tiene un ticket activo.");
        }

        // 2. Verificar si hay una Membresía activa
        var membership = await _membershipRepository.GetActiveMembershipByLicensePlateAsync(newTicket.LicensePlate);

        // 3. Inicializar el Ticket
        newTicket.EntryDateTime = DateTimeOffset.UtcNow;
        newTicket.Folio = $"F-{_folioCounter++}"; // Generar un folio simple
        newTicket.EntryType = membership != null ? EntryType.Membership : EntryType.Guest;
        newTicket.MembershipId = membership?.Id;
        newTicket.QRCode = newTicket.Folio; // Asumiendo que el QR es el Folio

        // 4. Crear el Ticket en la DB
        return await _ticketRepository.CreateAsync(newTicket);
    }
    
    // MÉTODO 2: REGISTRAR SALIDA (Cálculo y Finalización)

    public async Task<Ticket> RegisterExitAsync(string identifier, int exitOperatorId)
    {
        // 1. Buscar el ticket activo por placa o folio
        var ticket = await _ticketRepository.GetActiveTicketByLicensePlateAsync(identifier) ?? 
                     await _ticketRepository.GetActiveTicketByFolioAsync(identifier);
        
        if (ticket == null)
        {
            throw new KeyNotFoundException($"No se encontró un ticket activo para el identificador '{identifier}'.");
        }

        // 2. Registrar el tiempo de salida y el operador
        ticket.ExitDateTime = DateTimeOffset.UtcNow;
        ticket.ExitOperatorId = exitOperatorId;

        // 3. CALCULAR DURACIÓN
        var duration = ticket.ExitDateTime.Value - ticket.EntryDateTime;
        ticket.TotalMinutes = (int)Math.Ceiling(duration.TotalMinutes);

        // 4. CALCULAR COSTO (Solo si no es una Membresía)
        if (ticket.EntryType == EntryType.Guest)
        {
            // La lógica de cálculo es el corazón del negocio
            await CalculateTicketCost(ticket);
        }
        else
        {
            // Membresía: Costo Cero
            ticket.TotalCost = 0.00m;
        }

        // 5. Actualizar y guardar el registro final
        return await _ticketRepository.UpdateAsync(ticket);
    }
    
    // LÓGICA DE CÁLCULO DE COBRO (Método Privado)

    private async Task CalculateTicketCost(Ticket ticket)
    {
        var rate = await _rateRepository.GetActiveRateByVehicleTypeAsync(ticket.VehicleType);
        
        if (rate == null)
        {
            throw new InvalidOperationException($"No existe una tarifa activa para el tipo de vehículo {ticket.VehicleType}.");
        }
        
        var totalMinutes = ticket.TotalMinutes!.Value;
        var totalCost = 0.00m;
        
        // Aplicar Período de Gracia: Si la duración está dentro del período de gracia, el costo es 0.
        if (totalMinutes <= rate.GracePeriodMinutes)
        {
            ticket.RateApplied = 0.00m;
            ticket.TotalCost = 0.00m;
            return;
        }

        // 1. CALCULAR COSTO POR FRACCIÓN
        // Se cobra por cada fracción de 60 minutos.
        // Ejemplo: 75 minutos = 2 horas (fracciones)
        int billedHours = (int)Math.Ceiling(totalMinutes / 60.0);
        totalCost = billedHours * rate.HourlyRate;

        // 2. APLICAR LÍMITE DIARIO (Daily Cap)
        // Si el costo calculado excede el límite diario, se aplica el límite.
        if (totalCost > rate.DailyCap)
        {
            totalCost = rate.DailyCap;
        }

        // 3. Guardar resultados
        ticket.RateApplied = rate.HourlyRate; // Opcionalmente puedes guardar el rate.FractionRate
        ticket.TotalCost = totalCost;
    }

    // Método para ver el estado del ticket (útil para la aplicación Java)
    public async Task<Ticket?> GetTicketStatusAsync(string identifier)
    {
        return await _ticketRepository.GetActiveTicketByLicensePlateAsync(identifier) ?? 
               await _ticketRepository.GetActiveTicketByFolioAsync(identifier);
    }
}