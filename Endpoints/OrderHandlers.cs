using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommerce.Application.Orders;
using Ecommerce.Application.Orders.Dtos;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Endpoints;

public static class OrderHandlers
{
    public static async Task<IResult> CreateOrder(
        CreateOrderDto dto,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService,
        CancellationToken ct)
    {
        var userId = GetUserId(claimsPrincipal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var order = await orderService.CreateOrderAsync(userId.Value, dto, ct);
        return Results.Created($"/api/orders/{order.Id}", order);
    }

    public static async Task<IResult> GetMyOrders(
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = GetUserId(claimsPrincipal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var orders = await orderService.GetUserOrdersAsync(userId.Value, page, pageSize, ct);
        return Results.Ok(orders);
    }

    public static async Task<IResult> GetOrderById(
        Guid id,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService,
        CancellationToken ct)
    {
        var userId = GetUserId(claimsPrincipal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var isAdmin = claimsPrincipal.IsInRole(UserRole.Admin.ToString());
        var order = await orderService.GetOrderByIdAsync(id, userId.Value, isAdmin, ct);

        return order is null
            ? Results.NotFound(new { message = $"Order with ID '{id}' was not found." })
            : Results.Ok(order);
    }

    public static async Task<IResult> GetAllOrders(
        IOrderService orderService,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var orders = await orderService.GetAllOrdersAsync(page, pageSize, ct);
        return Results.Ok(orders);
    }

    public static async Task<IResult> UpdateOrderStatus(
        Guid id,
        UpdateOrderStatusDto dto,
        IOrderService orderService,
        CancellationToken ct)
    {
        var updatedOrder = await orderService.UpdateOrderStatusAsync(id, dto.Status, ct);

        return updatedOrder is null
            ? Results.NotFound(new { message = $"Order with ID '{id}' was not found." })
            : Results.Ok(updatedOrder);
    }

    public static async Task<IResult> CancelOrder(
        Guid id,
        ClaimsPrincipal claimsPrincipal,
        IOrderService orderService,
        CancellationToken ct)
    {
        var userId = GetUserId(claimsPrincipal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var isAdmin = claimsPrincipal.IsInRole(UserRole.Admin.ToString());
        var cancelled = await orderService.CancelOrderAsync(id, userId.Value, isAdmin, ct);

        return !cancelled
            ? Results.NotFound(new { message = $"Order with ID '{id}' was not found." })
            : Results.Ok(new { message = "Order successfully cancelled and stock restored." });
    }

    private static Guid? GetUserId(ClaimsPrincipal principal)
    {
        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(sub, out var guid) ? guid : null;
    }
}
