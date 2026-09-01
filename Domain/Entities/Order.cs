using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    // Nullable for Guest Checkout
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public required string CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public required string PhoneNumber { get; set; }
    public required string ShippingAddress { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }

    public List<OrderItem> Items { get; set; } = [];

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}
