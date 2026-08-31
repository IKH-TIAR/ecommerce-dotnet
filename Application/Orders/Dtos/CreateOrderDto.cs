namespace Ecommerce.Application.Orders.Dtos;

public record CreateOrderDto(
    string ShippingAddress,
    string PhoneNumber,
    List<CreateOrderItemDto> Items
);
