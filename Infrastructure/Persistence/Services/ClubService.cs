using Ecommerce.Application.Clubs;
using Ecommerce.Application.Clubs.Dtos;
using Ecommerce.Application.Common.Models;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Services;


public class ClubService(AppDbContext dbContext) : IClubService
{
    public async Task<ClubDto> CreateClubAsync(CreateClubDto dto, CancellationToken cancellationToken = default)
    {
        var club = new Club
        {
            Name = dto.Name,
            Country = dto.Country,
            League = dto.League,
            LogoUrl = dto.LogoUrl
        };

        await dbContext.Clubs.AddAsync(club, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ClubDto(
            club.Id,
            club.Name,
            club.Country,
            club.League,
            club.LogoUrl,
            club.CreatedAt,
            club.UpdatedAt
        );
    }

    public async Task<PagedResult<ClubDto>> GetAllClubAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
       page = page < 1 ? 1 : page;
       pageSize = pageSize < 1 ? 1 : (pageSize > 100 ? 100 : pageSize);

       var totalCount = await dbContext.Clubs.CountAsync(cancellationToken);

       var items = await dbContext.Clubs
       .AsNoTracking()
       .OrderByDescending(c => c.CreatedAt)
       .Skip((page - 1) * pageSize)
       .Take(pageSize)
       .Select(c => new ClubDto(
        c.Id,
        c.Name,
        c.Country,
        c.League,
        c.LogoUrl,
        c.CreatedAt,
        c.UpdatedAt
       ))
       .ToListAsync(cancellationToken);

       return PagedResult<ClubDto>.Create(items, totalCount, page, pageSize);
    }
}