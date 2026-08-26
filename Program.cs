using Ecommerce.Application.Jerseys;
using Ecommerce.Endpoints;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IJerseyService, JerseyService>();

builder.Services.AddValidatorsFromAssemblyContaining<IJerseyService>();

var app = builder.Build();

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

app.Run();
