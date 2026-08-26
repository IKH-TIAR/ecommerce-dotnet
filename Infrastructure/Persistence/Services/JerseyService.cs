using Ecommerce.Application.Jerseys;
using Ecommerce.Application.Jerseys.Dtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Services;

public class JerseyService(AppDbContext dbContext) : IJerseyService
{
    public async Task<JerseyDto> CreateJerseyAsync(CreateJerseyDto dto, CancellationToken cancellationToken = default)
    {
        var jersey = new Jersey
        {
            Name = dto.Name,
            Club = dto.Club,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrls = dto.ImageUrls,
            StockQuantity = dto.StockQuantity
        };

        await dbContext.jerseys.AddAsync(jersey, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new JerseyDto(
            jersey.Id,                                                                     
            jersey.Name,                                                                   
            jersey.Club,                                                                   
            jersey.Description,                                                            
            jersey.ImageUrls,                                                              
            jersey.Price,                                                                  
            jersey.StockQuantity,                                                          
            jersey.CreatedAt,                                                              
            jersey.UpdatedAt
        );
    }

    public async Task<List<JerseyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
       return await dbContext.jerseys.AsNoTracking().Select(j => new JerseyDto(
        j.Id,
        j.Name,
        j.Club,
        j.Description,
        j.ImageUrls,
        j.Price,
        j.StockQuantity,
        j.CreatedAt,
        j.UpdatedAt
       ))
       .ToListAsync(cancellationToken);
    }
}
