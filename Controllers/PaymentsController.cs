using CrudPark.API.Models;
using CrudPark.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudPark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET: api/payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }

    // GET: api/payments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPaymentById(int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        
        if (payment == null)
            return NotFound(new { message = $"No se encontró el pago con ID {id}" });
        
        return Ok(payment);
    }

    // GET: api/payments/by-ticket/5
    [HttpGet("by-ticket/{ticketId}")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsByTicket(int ticketId)
    {
        var payments = await _paymentService.GetPaymentsByTicketIdAsync(ticketId);
        return Ok(payments);
    }

    // GET: api/payments/by-operator/5
    [HttpGet("by-operator/{operatorId}")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsByOperator(int operatorId)
    {
        var payments = await _paymentService.GetPaymentsByOperatorIdAsync(operatorId);
        return Ok(payments);
    }

    // GET: api/payments/by-date-range?startDate=2024-10-01&endDate=2024-10-31
    [HttpGet("by-date-range")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(new { message = "La fecha de inicio debe ser anterior a la fecha de fin" });

        var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
        return Ok(payments);
    }

    // POST: api/payments
    [HttpPost]
    public async Task<ActionResult<Payment>> CreatePayment([FromBody] Payment payment)
    {
        try
        {
            var created = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/payments/calculate/5
    [HttpGet("calculate/{ticketId}")]
    public async Task<ActionResult<decimal>> CalculatePaymentAmount(int ticketId)
    {
        try
        {
            var amount = await _paymentService.CalculatePaymentAmountAsync(ticketId);
            return Ok(new { 
                ticketId = ticketId,
                amount = amount,
                message = amount == 0 ? "No se requiere pago" : "Monto calculado exitosamente"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/payments/daily-revenue?date=2024-10-21
    [HttpGet("daily-revenue")]
    public async Task<ActionResult<decimal>> GetDailyRevenue([FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.Now.Date;
        var revenue = await _paymentService.GetDailyRevenueAsync(targetDate);
        
        return Ok(new {
            date = targetDate.ToString("yyyy-MM-dd"),
            revenue = revenue
        });
    }

    // GET: api/payments/revenue-range?startDate=2024-10-01&endDate=2024-10-31
    [HttpGet("revenue-range")]
    public async Task<ActionResult<decimal>> GetRevenueByRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(new { message = "La fecha de inicio debe ser anterior a la fecha de fin" });

        var revenue = await _paymentService.GetRevenueByDateRangeAsync(startDate, endDate);
        
        return Ok(new {
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd"),
            revenue = revenue
        });
    }
}