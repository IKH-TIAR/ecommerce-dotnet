using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders.Dtos;

public record UpdateOrderStatusDto(
    OrderStatus Status
);
