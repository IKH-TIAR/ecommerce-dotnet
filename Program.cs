using System.Diagnostics;
using Ecommerce.Application.Auth;
using Ecommerce.Application.Clubs;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Jerseys;
using Ecommerce.Endpoints;
using Ecommerce.Infrastructure.Exceptions;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Persistence.Services;
using Ecommerce.Infrastructure.Security;
using Ecommerce.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["traceId"] = traceId;
        context.ProblemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddSingleton<IPasswordHasher, PasswordHash>();  
builder.Services.AddScoped<IAuthService, AuthService>(); 

builder.Services.AddScoped<IJerseyService, JerseyService>();
builder.Services.AddScoped<IClubService, ClubService>();

builder.Services.AddValidatorsFromAssemblyContaining<IJerseyService>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())                                                      
{                                                                                          
    app.UseHttpsRedirection();                                                             
}


app.MapJerseyEndpoints();
app.MapClubEndpoints();
app.MapAuthEndpoints();

app.Run();
