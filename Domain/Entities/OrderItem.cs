using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid JerseyId { get; set; }
    public Jersey? Jersey { get; set; }

    public JerseySize Size { get; set; } = JerseySize.M;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Computed total price for this line item
    public decimal TotalPrice => Quantity * UnitPrice;
}
