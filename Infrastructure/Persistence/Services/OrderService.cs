
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders;
using Ecommerce.Application.Orders.Dtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Services;

public class OrderService(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderDto> CreateOrderAsync(Guid? userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // If user is not logged in, but provided an optional Password & Email -> Auto-create account during checkout!
            if (userId is null && !string.IsNullOrWhiteSpace(dto.Password) && !string.IsNullOrWhiteSpace(dto.CustomerEmail))
            {
                var normalizedEmail = dto.CustomerEmail.Trim().ToLowerInvariant();
                var userExists = await dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

                if (!userExists)
                {
                    var newUser = new User
                    {
                        FullName = dto.CustomerName.Trim(),
                        Email = normalizedEmail,
                        PasswordHash = passwordHasher.HashPassword(dto.Password),
                        Role = UserRole.Customer
                    };

                    await dbContext.Users.AddAsync(newUser, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    userId = newUser.Id;

                    logger.LogInformation("Instant account created for guest user {UserId} ({Email}) during checkout", newUser.Id, newUser.Email);
                }
            }

            var jerseyIds = dto.Items.Select(i => i.JerseyId).Distinct().ToList();

            var jerseys = await dbContext.Jerseys
                .AsNoTracking()
                .Where(j => jerseyIds.Contains(j.Id))
                .ToDictionaryAsync(j => j.Id, cancellationToken);

            foreach (var item in dto.Items)
            {
                if (!jerseys.TryGetValue(item.JerseyId, out var jersey))
                {
                    logger.LogWarning("Checkout failed: Jersey {JerseyId} does not exist", item.JerseyId);
                    throw new BadHttpRequestException($"Jersey with ID '{item.JerseyId}' does not exist.");
                }
            }

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var jersey = jerseys[item.JerseyId];

                var affectedSizeRows = await dbContext.JerseySizeStocks
                    .Where(s => s.JerseyId == item.JerseyId && s.Size == item.Size && s.StockQuantity >= item.Quantity)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(s => s.StockQuantity, s => s.StockQuantity - item.Quantity),
                        cancellationToken);

                if (affectedSizeRows == 0)
                {
                    logger.LogWarning("Checkout failed: Insufficient stock for {JerseyName} ({JerseyId}) Size {Size}.",
                        jersey.Name, jersey.Id, item.Size);
                    throw new BadHttpRequestException($"Insufficient stock for '{jersey.Name}' in Size {item.Size}. The selected size may be out of stock.");
                }

                await dbContext.Jerseys
                    .Where(j => j.Id == item.JerseyId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(j => j.StockQuantity, j => j.StockQuantity - item.Quantity)
                        .SetProperty(j => j.UpdatedAt, DateTimeOffset.UtcNow),
                        cancellationToken);

                var unitPrice = jersey.Price;
                totalAmount += unitPrice * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    JerseyId = jersey.Id,
                    Size = item.Size,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            var order = new Order
            {
                UserId = userId,
                CustomerName = dto.CustomerName.Trim(),
                CustomerEmail = dto.CustomerEmail?.Trim().ToLowerInvariant(),
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,
                ShippingAddress = dto.ShippingAddress.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Items = orderItems
            };

            await dbContext.Orders.AddAsync(order, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Order {OrderId} placed for Customer '{CustomerName}' (UserId: {UserId}) with Total {TotalAmount} BDT",
                order.Id, order.CustomerName, userId, totalAmount);

            var itemDtos = order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.JerseyId,
                jerseys[i.JerseyId].Name,
                i.Size,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList();

            return new OrderDto(
                order.Id,
                order.UserId,
                order.CustomerName,
                order.CustomerEmail,
                order.Status,
                order.TotalAmount,
                order.ShippingAddress,
                order.PhoneNumber,
                itemDtos,
                order.CreatedAt,
                order.UpdatedAt
            );
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
                o.CustomerName,
                o.CustomerEmail,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PhoneNumber,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.JerseyId,
                    i.Jersey != null ? i.Jersey.Name : "Jersey",
                    i.Size,
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

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId, Guid? userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId);

        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        return await query
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.CustomerName,
                o.CustomerEmail,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PhoneNumber,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.JerseyId,
                    i.Jersey != null ? i.Jersey.Name : "Jersey",
                    i.Size,
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
                o.CustomerName,
                o.CustomerEmail,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PhoneNumber,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.JerseyId,
                    i.Jersey != null ? i.Jersey.Name : "Jersey",
                    i.Size,
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
            i.Size,
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice
        )).ToList();

        return new OrderDto(
            order.Id,
            order.UserId,
            order.CustomerName,
            order.CustomerEmail,
            order.Status,
            order.TotalAmount,
            order.ShippingAddress,
            order.PhoneNumber,
            itemDtos,
            order.CreatedAt,
            order.UpdatedAt
        );
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, Guid? userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return false;
        }

        if (!isAdmin && userId.HasValue && order.UserId != userId.Value)
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
            await dbContext.JerseySizeStocks
                .Where(s => s.JerseyId == item.JerseyId && s.Size == item.Size)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.StockQuantity, s => s.StockQuantity + item.Quantity),
                    cancellationToken);

            await dbContext.Jerseys
                .Where(j => j.Id == item.JerseyId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(j => j.StockQuantity, j => j.StockQuantity + item.Quantity)
                    .SetProperty(j => j.UpdatedAt, DateTimeOffset.UtcNow),
                    cancellationToken);
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} successfully cancelled. Stock restored for {ItemCount} items across their specific sizes.",
            order.Id, order.Items.Count);

        return true;
    }
}
