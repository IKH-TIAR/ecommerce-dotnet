using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Dtos;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid? userId, CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<OrderDto>> GetUserOrdersAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetOrderByIdAsync(Guid orderId, Guid? userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<OrderDto?> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);
    Task<bool> CancelOrderAsync(Guid orderId, Guid? userId, bool isAdmin, CancellationToken cancellationToken = default);
}
