using Ecommerce.Application.Common.Models;
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

        await dbContext.Jerseys.AddAsync(jersey, cancellationToken);
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

    public async Task<JerseyDto?> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return await dbContext.Jerseys
        .AsNoTracking()
        .Where(j => j.Id == Id)
        .Select(j => new JerseyDto(
            j.Id,
            j.Name,                                                                    
            j.Club,                                                                    
            j.Description,                                                             
            j.ImageUrls,                                                               
            j.Price,                                                                   
            j.StockQuantity,                                                           
            j.CreatedAt,                                                               
            j.UpdatedAt
        )).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<JerseyDto>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var totalCount = await dbContext.Jerseys.CountAsync(cancellationToken);

       var items = await dbContext.Jerseys
       .AsNoTracking()
       .OrderByDescending(j => j.CreatedAt)
       .Skip((page - 1) * pageSize)
       .Take(pageSize)
       .Select(j => new JerseyDto(
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

       return PagedResult<JerseyDto>.Create(items, totalCount, page, pageSize);
    }

    public async Task<JerseyDto?> UpdateJerseyAsync(Guid Id, UpdateJerseyDto dto, CancellationToken cancellationToken = default)
    {
        var jersey = await dbContext.Jerseys.FirstOrDefaultAsync(j => j.Id == Id, cancellationToken);
        if (jersey is null)
        {
            return null;
        }
        if (dto.Name is not null) jersey.Name = dto.Name;
        if (dto.Club is not null) jersey.Club = dto.Club; 
        if (dto.Description is not null) jersey.Description = dto.Description;                          
        if (dto.ImageUrls is not null) jersey.ImageUrls = dto.ImageUrls;                                          
        if (dto.Price.HasValue) jersey.Price = dto.Price.Value;                                                  
        if (dto.StockQuantity.HasValue) jersey.StockQuantity = dto.StockQuantity.Value;                
                                                          
        // 4. Update the modified timestamp               
        jersey.UpdatedAt = DateTimeOffset.UtcNow;

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

    public async Task<bool> DeleteJerseyAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        var deleteRowCount = await dbContext.Jerseys
        .Where(j => j.Id == Id)
        .ExecuteDeleteAsync(cancellationToken);

        return deleteRowCount > 0;
    }
}
