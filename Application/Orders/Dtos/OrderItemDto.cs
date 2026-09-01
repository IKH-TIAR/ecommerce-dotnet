using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders.Dtos;

public record OrderItemDto(
    Guid Id,
    Guid JerseyId,
    string JerseyName,
    JerseySize Size,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
