using Ecommerce.Application.Jerseys;
using Ecommerce.Application.Jerseys.Dtos;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Services;

public class JerseyService(AppDbContext dbContext) : IJerseyService
{
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
