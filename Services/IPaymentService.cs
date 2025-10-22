using CrudPark.API.Models;

namespace CrudPark.API.Services;

public interface IPaymentService
{
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task<Payment?> GetPaymentByIdAsync(int id);
    Task<IEnumerable<Payment>> GetPaymentsByTicketIdAsync(int ticketId);
    Task<IEnumerable<Payment>> GetPaymentsByOperatorIdAsync(int operatorId);
    Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<decimal> CalculatePaymentAmountAsync(int ticketId);
    Task<decimal> GetDailyRevenueAsync(DateTime date);
    Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
}