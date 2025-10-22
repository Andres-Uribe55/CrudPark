using CrudPark.API.Models;
using CrudPark.API.Repositories;

namespace CrudPark.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IRateRepository _rateRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        ITicketRepository ticketRepository,
        IRateRepository rateRepository)
    {
        _paymentRepository = paymentRepository;
        _ticketRepository = ticketRepository;
        _rateRepository = rateRepository;
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _paymentRepository.GetAllAsync();
    }

    public async Task<Payment?> GetPaymentByIdAsync(int id)
    {
        return await _paymentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByTicketIdAsync(int ticketId)
    {
        return await _paymentRepository.GetByTicketIdAsync(ticketId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByOperatorIdAsync(int operatorId)
    {
        return await _paymentRepository.GetByOperatorIdAsync(operatorId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        // Validar que el ticket existe
        var ticket = await _ticketRepository.GetByIdAsync(payment.TicketId);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"No se encontró el ticket con ID {payment.TicketId}");
        }

        // Validar que el ticket ya tiene salida registrada
        if (ticket.ExitDateTime == null)
        {
            throw new InvalidOperationException("No se puede registrar un pago para un ticket sin salida registrada");
        }

        // Validar que no exista ya un pago para este ticket
        var existingPayments = await _paymentRepository.GetByTicketIdAsync(payment.TicketId);
        if (existingPayments.Any())
        {
            throw new InvalidOperationException($"Ya existe un pago registrado para el ticket {payment.TicketId}");
        }

        // Establecer fecha/hora actual
        payment.PaymentDateTime = DateTimeOffset.Now;

        return await _paymentRepository.CreateAsync(payment);
    }

    public async Task<decimal> CalculatePaymentAmountAsync(int ticketId)
    {
        // Obtener el ticket
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"No se encontró el ticket con ID {ticketId}");
        }

        // Validar que tenga entrada y salida
        if (ticket.ExitDateTime == null)
        {
            throw new InvalidOperationException("El ticket no tiene salida registrada");
        }

        // Si es mensualidad vigente, no se cobra
        if (ticket.EntryType == EntryType.Membership && ticket.MembershipId.HasValue)
        {
            return 0;
        }

        // Calcular tiempo total en minutos
        var totalMinutes = (int)(ticket.ExitDateTime.Value - ticket.EntryDateTime).TotalMinutes;
        ticket.TotalMinutes = totalMinutes;
        await _ticketRepository.UpdateAsync(ticket);

        // Obtener la tarifa activa para el tipo de vehículo
        var rate = await _rateRepository.GetActiveRateByVehicleTypeAsync(ticket.VehicleType);
        if (rate == null)
        {
            throw new InvalidOperationException($"No se encontró una tarifa activa para el tipo de vehículo {ticket.VehicleType}");
        }

        // Aplicar tiempo de gracia
        if (totalMinutes <= rate.GracePeriodMinutes)
        {
            return 0;
        }

        // Calcular minutos cobrables (después del tiempo de gracia)
        var chargeableMinutes = totalMinutes - rate.GracePeriodMinutes;

        // Calcular cobro
        decimal amount = 0;

        // Calcular horas completas
        var fullHours = chargeableMinutes / 60;
        amount += fullHours * rate.HourlyRate;

        // Calcular fracción restante
        var remainingMinutes = chargeableMinutes % 60;
        if (remainingMinutes > 0)
        {
            amount += rate.FractionRate;
        }

        // Aplicar tope diario si existe
        if (rate.DailyCap > 0 && amount > rate.DailyCap)
        {
            amount = rate.DailyCap;
        }

        return amount;
    }

    public async Task<decimal> GetDailyRevenueAsync(DateTime date)
    {
        return await _paymentRepository.GetTotalRevenueByDateAsync(date);
    }

    public async Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _paymentRepository.GetTotalRevenueByDateRangeAsync(startDate, endDate);
    }
}