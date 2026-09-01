using Ecommerce.Application.Clubs.Dtos;
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
        var clubExists = await dbContext.Clubs.AnyAsync(
            c => c.Id == dto.ClubId, cancellationToken
        );

        if (!clubExists)
        {
            throw new BadHttpRequestException($"Club with ID '{dto.ClubId}' does not exist.");
        }

        var totalStock = dto.Sizes.Sum(s => s.StockQuantity);

        var sizeEntities = dto.Sizes.Select(s => new JerseySizeStock
        {
            Size = s.Size,
            StockQuantity = s.StockQuantity
        }).ToList();

        var jersey = new Jersey
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrls = dto.ImageUrls,
            StockQuantity = totalStock,
            ClubId = dto.ClubId,
            Sizes = sizeEntities
        };

        await dbContext.Jerseys.AddAsync(jersey, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var sizeDtos = jersey.Sizes
            .OrderBy(s => s.Size)
            .Select(s => new JerseySizeStockDto(s.Size, s.StockQuantity))
            .ToList();

        return new JerseyDto(
            jersey.Id,
            jersey.Name,
            jersey.Description,
            jersey.ImageUrls,
            jersey.Price,
            jersey.StockQuantity,
            jersey.ClubId,
            null,
            jersey.CreatedAt,
            jersey.UpdatedAt,
            sizeDtos
        );
    }

    public async Task<JerseyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Jerseys
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new JerseyDto(
                j.Id,
                j.Name,
                j.Description,
                j.ImageUrls,
                j.Price,
                j.StockQuantity,
                j.ClubId,
                j.Club == null ? null : new ClubDto(j.Club.Id, j.Club.Name, j.Club.Country, j.Club.League, j.Club.LogoUrl, j.Club.CreatedAt, j.Club.UpdatedAt),
                j.CreatedAt,
                j.UpdatedAt,
                j.Sizes.OrderBy(s => s.Size).Select(s => new JerseySizeStockDto(s.Size, s.StockQuantity)).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<JerseyDto>> GetAllAsync(GetJerseysQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);

        var queryable = dbContext.Jerseys
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim();
            queryable = queryable.Where(j =>
                EF.Functions.ILike(j.Name, $"%{term}%") ||
                (j.Description != null && EF.Functions.ILike(j.Description, $"%{term}%")));
        }

        if (query.ClubId.HasValue)
        {
            queryable = queryable.Where(j => j.ClubId == query.ClubId.Value);
        }

        if (query.MinPrice.HasValue)
        {
            queryable = queryable.Where(j => j.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            queryable = queryable.Where(j => j.Price <= query.MaxPrice.Value);
        }

        if (query.InStockOnly == true)
        {
            queryable = queryable.Where(j => j.StockQuantity > 0);
        }

        var isDescending = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        queryable = (query.SortBy?.Trim().ToLowerInvariant()) switch
        {
            "price" => isDescending ? queryable.OrderByDescending(j => j.Price) : queryable.OrderBy(j => j.Price),
            "name" => isDescending ? queryable.OrderByDescending(j => j.Name) : queryable.OrderBy(j => j.Name),
            _ => isDescending ? queryable.OrderByDescending(j => j.CreatedAt) : queryable.OrderBy(j => j.CreatedAt)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JerseyDto(
                j.Id,
                j.Name,
                j.Description,
                j.ImageUrls,
                j.Price,
                j.StockQuantity,
                j.ClubId,
                j.Club == null ? null : new ClubDto(j.Club.Id, j.Club.Name, j.Club.Country, j.Club.League, j.Club.LogoUrl, j.Club.CreatedAt, j.Club.UpdatedAt),
                j.CreatedAt,
                j.UpdatedAt,
                j.Sizes.OrderBy(s => s.Size).Select(s => new JerseySizeStockDto(s.Size, s.StockQuantity)).ToList()
            ))
            .ToListAsync(cancellationToken);

        return PagedResult<JerseyDto>.Create(items, totalCount, page, pageSize);
    }

    public async Task<JerseyDto?> UpdateJerseyAsync(Guid id, UpdateJerseyDto dto, CancellationToken cancellationToken = default)
    {
        var jersey = await dbContext.Jerseys
            .Include(j => j.Sizes)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

        if (jersey is null)
        {
            return null;
        }

        if (dto.Name is not null) jersey.Name = dto.Name;
        if (dto.Description is not null) jersey.Description = dto.Description;
        if (dto.ImageUrls is not null) jersey.ImageUrls = dto.ImageUrls;
        if (dto.Price.HasValue) jersey.Price = dto.Price.Value;

        if (dto.ClubId.HasValue)
        {
            var clubExists = await dbContext.Clubs.AnyAsync(
                c => c.Id == dto.ClubId.Value, cancellationToken
            );

            if (!clubExists)
            {
                throw new BadHttpRequestException($"Club with ID '{dto.ClubId.Value}' does not exist.");
            }

            jersey.ClubId = dto.ClubId.Value;
        }

        if (dto.Sizes is not null)
        {
            foreach (var sizeDto in dto.Sizes)
            {
                var existing = jersey.Sizes.FirstOrDefault(s => s.Size == sizeDto.Size);
                if (existing is not null)
                {
                    existing.StockQuantity = sizeDto.StockQuantity;
                }
                else
                {
                    jersey.Sizes.Add(new JerseySizeStock
                    {
                        JerseyId = jersey.Id,
                        Size = sizeDto.Size,
                        StockQuantity = sizeDto.StockQuantity
                    });
                }
            }

            jersey.StockQuantity = jersey.Sizes.Sum(s => s.StockQuantity);
        }

        jersey.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        var sizeDtos = jersey.Sizes
            .OrderBy(s => s.Size)
            .Select(s => new JerseySizeStockDto(s.Size, s.StockQuantity))
            .ToList();

        return new JerseyDto(
            jersey.Id,
            jersey.Name,
            jersey.Description,
            jersey.ImageUrls,
            jersey.Price,
            jersey.StockQuantity,
            jersey.ClubId,
            null,
            jersey.CreatedAt,
            jersey.UpdatedAt,
            sizeDtos
        );
    }

    public async Task<bool> DeleteJerseyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleteRowCount = await dbContext.Jerseys
            .Where(j => j.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleteRowCount > 0;
    }
}
