using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders.Dtos;

public record OrderDto(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalAmount,
    string ShippingAddress,
    string PhoneNumber,
    List<OrderItemDto> Items,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
