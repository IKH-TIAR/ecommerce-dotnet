using Ecommerce.Application.Common.Security;
using Ecommerce.Application.Orders.Dtos;
using Ecommerce.Endpoints.Filters;

namespace Ecommerce.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        // Customer Routes (Requires Any Logged-in Customer/User)
        group.MapPost("/", OrderHandlers.CreateOrder)
            .WithValidation<CreateOrderDto>()
            .RequireAuthorization();

        group.MapGet("/my-orders", OrderHandlers.GetMyOrders)
            .RequireAuthorization();

        group.MapGet("/{id:guid}", OrderHandlers.GetOrderById)
            .RequireAuthorization();

        group.MapPost("/{id:guid}/cancel", OrderHandlers.CancelOrder)
            .RequireAuthorization();

        // Admin Only Routes
        group.MapGet("/", OrderHandlers.GetAllOrders)
            .RequireAuthorization(Policies.AdminOnly);

        group.MapPatch("/{id:guid}/status", OrderHandlers.UpdateOrderStatus)
            .WithValidation<UpdateOrderStatusDto>()
            .RequireAuthorization(Policies.AdminOnly);
    }
}
