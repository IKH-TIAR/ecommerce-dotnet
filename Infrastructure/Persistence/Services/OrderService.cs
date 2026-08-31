using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders;
using Ecommerce.Application.Orders.Dtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Services;

public class OrderService(
    AppDbContext dbContext,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var jerseyIds = dto.Items.Select(i => i.JerseyId).Distinct().ToList();

        var jerseys = await dbContext.Jerseys
            .Where(j => jerseyIds.Contains(j.Id))
            .ToDictionaryAsync(j => j.Id, cancellationToken);

        foreach (var item in dto.Items)
        {
            if (!jerseys.TryGetValue(item.JerseyId, out var jersey))
            {
                logger.LogWarning("Checkout failed: Jersey {JerseyId} does not exist", item.JerseyId);
                throw new BadHttpRequestException($"Jersey with ID '{item.JerseyId}' does not exist.");
            }

            if (jersey.StockQuantity < item.Quantity)
            {
                logger.LogWarning("Checkout failed: Insufficient stock for {JerseyName} ({JerseyId}). Available: {Available}, Requested: {Requested}",
                    jersey.Name, jersey.Id, jersey.StockQuantity, item.Quantity);
                throw new BadHttpRequestException($"Insufficient stock for '{jersey.Name}'. Available: {jersey.StockQuantity}, Requested: {item.Quantity}.");
            }
        }

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var jersey = jerseys[item.JerseyId];

            jersey.StockQuantity -= item.Quantity;
            jersey.UpdatedAt = DateTimeOffset.UtcNow;

            var unitPrice = jersey.Price;
            totalAmount += unitPrice * item.Quantity;

            orderItems.Add(new OrderItem
            {
                JerseyId = jersey.Id,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });
        }

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            ShippingAddress = dto.ShippingAddress.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Items = orderItems
        };

        await dbContext.Orders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} successfully placed by User {UserId} with {ItemCount} items for Total {TotalAmount} BDT",
            order.Id, userId, order.Items.Count, totalAmount);

        var itemDtos = order.Items.Select(i => new OrderItemDto(
            i.Id,
            i.JerseyId,
            jerseys[i.JerseyId].Name,
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice
        )).ToList();

        return new OrderDto(
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount,
            order.ShippingAddress,
            order.PhoneNumber,
            itemDtos,
            order.CreatedAt,
            order.UpdatedAt
        );
    }

    public async Task<PagedResult<OrderDto>> GetUserOrdersAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var query = dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PhoneNumber,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.JerseyId,
                    i.Jersey != null ? i.Jersey.Name : "Jersey",
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice
                )).ToList(),
                o.CreatedAt,
                o.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return PagedResult<OrderDto>.Create(items, totalCount, page, pageSize);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId);

        if (!isAdmin)
        {
            query = query.Where(o => o.UserId == userId);
        }

        return await query
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PhoneNumber,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.JerseyId,
                    i.Jersey != null ? i.Jersey.Name : "Jersey",
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice
                )).ToList(),
                o.CreatedAt,
                o.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var query = dbContext.Orders.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PhoneNumber,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.JerseyId,
                    i.Jersey != null ? i.Jersey.Name : "Jersey",
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice
                )).ToList(),
                o.CreatedAt,
                o.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return PagedResult<OrderDto>.Create(items, totalCount, page, pageSize);
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Jersey)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var previousStatus = order.Status;
        order.Status = newStatus;
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} status changed from {PreviousStatus} to {NewStatus}",
            order.Id, previousStatus, newStatus);

        var itemDtos = order.Items.Select(i => new OrderItemDto(
            i.Id,
            i.JerseyId,
            i.Jersey != null ? i.Jersey.Name : "Jersey",
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice
        )).ToList();

        return new OrderDto(
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount,
            order.ShippingAddress,
            order.PhoneNumber,
            itemDtos,
            order.CreatedAt,
            order.UpdatedAt
        );
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Jersey)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return false;
        }

        if (!isAdmin && order.UserId != userId)
        {
            logger.LogWarning("Unauthorized cancellation attempt for Order {OrderId} by User {UserId}", orderId, userId);
            throw new BadHttpRequestException("You are not authorized to cancel this order.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new BadHttpRequestException("This order is already cancelled.");
        }

        if (order.Status is OrderStatus.Shipped or OrderStatus.Delivered)
        {
            throw new BadHttpRequestException($"Cannot cancel an order that has already been {order.Status.ToString().ToLowerInvariant()}.");
        }

        foreach (var item in order.Items)
        {
            if (item.Jersey is not null)
            {
                item.Jersey.StockQuantity += item.Quantity;
                item.Jersey.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} successfully cancelled by User {UserId}. Stock restored for {ItemCount} items.",
            order.Id, userId, order.Items.Count);

        return true;
    }
}
