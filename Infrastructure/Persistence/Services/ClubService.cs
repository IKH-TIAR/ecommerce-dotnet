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
        var nameExists = await dbContext.Clubs
            .AnyAsync(c => c.Name == dto.Name, cancellationToken);

        if (nameExists)
        {
            throw new BadHttpRequestException($"A club with name '{dto.Name}' already exists.");
        }

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

    public async Task<ClubDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ClubDto(
                c.Id,
                c.Name,
                c.Country,
                c.League,
                c.LogoUrl,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<ClubDto>> GetAllClubAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

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

    public async Task<ClubDto?> UpdateClubAsync(Guid id, UpdateClubDto dto, CancellationToken cancellationToken = default)
    {
        var club = await dbContext.Clubs.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (club is null)
        {
            return null;
        }

        if (dto.Name is not null && dto.Name != club.Name)
        {
            var nameExists = await dbContext.Clubs
                .AnyAsync(c => c.Name == dto.Name && c.Id != id, cancellationToken);

            if (nameExists)
            {
                throw new BadHttpRequestException($"A club with name '{dto.Name}' already exists.");
            }

            club.Name = dto.Name;
        }

        if (dto.Country is not null) club.Country = dto.Country;
        if (dto.League is not null) club.League = dto.League;
        if (dto.LogoUrl is not null) club.LogoUrl = dto.LogoUrl;

        club.UpdatedAt = DateTimeOffset.UtcNow;

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

    public async Task<bool> DeleteClubAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var clubExists = await dbContext.Clubs.AnyAsync(c => c.Id == id, cancellationToken);
        if (!clubExists)
        {
            return false;
        }

        // Check if any jerseys are still attached to this club
        var hasJerseys = await dbContext.Jerseys.AnyAsync(j => j.ClubId == id, cancellationToken);
        if (hasJerseys)
        {
            throw new BadHttpRequestException("Cannot delete this club because jerseys are currently linked to it. Please reassign or delete the jerseys first.");
        }

        var deletedRows = await dbContext.Clubs
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedRows > 0;
    }
}