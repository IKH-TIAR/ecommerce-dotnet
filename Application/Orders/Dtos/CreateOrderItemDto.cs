namespace Ecommerce.Application.Orders.Dtos;

public record CreateOrderItemDto(
    Guid JerseyId,
    int Quantity
);
