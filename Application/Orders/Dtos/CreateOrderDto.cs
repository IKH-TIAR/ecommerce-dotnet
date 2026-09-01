namespace Ecommerce.Application.Orders.Dtos;

public record CreateOrderDto(
    string CustomerName,
    string? CustomerEmail,
    string? Password,
    string ShippingAddress,
    string PhoneNumber,
    List<CreateOrderItemDto> Items
);
