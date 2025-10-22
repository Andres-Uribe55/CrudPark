using CrudPark.API.Models;

namespace CrudPark.API.Repositories;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetByTicketIdAsync(int ticketId);
    Task<IEnumerable<Payment>> GetByOperatorIdAsync(int operatorId);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Payment> CreateAsync(Payment payment);
    Task<decimal> GetTotalRevenueByDateAsync(DateTime date);
    Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
}