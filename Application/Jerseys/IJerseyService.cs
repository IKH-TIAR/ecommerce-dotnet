using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Jerseys.Dtos;

namespace Ecommerce.Application.Jerseys;

public interface IJerseyService
{
    Task<PagedResult<JerseyDto>> GetAllAsync(GetJerseysQuery query, CancellationToken cancellationToken = default);
    Task<JerseyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JerseyDto?> UpdateJerseyAsync(Guid id, UpdateJerseyDto dto, CancellationToken cancellationToken = default);
    Task<JerseyDto> CreateJerseyAsync(CreateJerseyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteJerseyAsync(Guid id, CancellationToken cancellationToken = default);
}