using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders.Dtos;

public record CreateOrderItemDto(
    Guid JerseyId,
    JerseySize Size,
    int Quantity
);
