namespace Ecommerce.Application.Orders.Dtos;

public record OrderItemDto(
    Guid Id,
    Guid JerseyId,
    string JerseyName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
