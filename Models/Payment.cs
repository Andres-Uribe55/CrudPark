namespace CrudPark.API.Models;

public class Payment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public decimal AmountCharged { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDateTime { get; set; }
    public int OperatorId { get; set; }
    
    // Navigation Properties
    public Ticket? Ticket { get; set; }
    public Operator? Operator { get; set; }
}

public enum PaymentMethod
{
    Cash,
    Card,
    Transfer
}