using Microsoft.AspNetCore.Http.Features;

namespace Ecommerce.Application.Common.Models;

public record PagedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPage,
    bool HasPreviousPage,
    bool HasNextPage
)
{
    public static PagedResult<T> Create(List<T> items, int totalCount, int page, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<T>(
            Items: items,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPage: totalPages,
            HasPreviousPage: page > 1,
            HasNextPage: page < totalPages
        );
    }
}