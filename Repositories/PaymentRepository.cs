using CrudPark.API.Data;
using CrudPark.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudPark.API.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.Operator)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.Operator)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetByTicketIdAsync(int ticketId)
    {
        return await _context.Payments
            .Include(p => p.Operator)
            .Where(p => p.TicketId == ticketId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByOperatorIdAsync(int operatorId)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Where(p => p.OperatorId == operatorId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.Operator)
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<decimal> GetTotalRevenueByDateAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.Payments
            .Where(p => p.PaymentDateTime >= startOfDay && p.PaymentDateTime < endOfDay)
            .SumAsync(p => p.AmountCharged);
    }

    public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .SumAsync(p => p.AmountCharged);
    }
}