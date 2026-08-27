using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Jerseys.Dtos;

namespace Ecommerce.Application.Jerseys;

public interface IJerseyService
{
    Task<PagedResult<JerseyDto>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<JerseyDto?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<JerseyDto?> UpdateJerseyAsync(Guid Id, UpdateJerseyDto dto, CancellationToken cancellationToken = default);
    Task<JerseyDto> CreateJerseyAsync(CreateJerseyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteJerseyAsync(Guid Id, CancellationToken cancellationToken = default);

}