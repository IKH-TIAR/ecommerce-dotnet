using Ecommerce.Application.Jerseys.Dtos;

namespace Ecommerce.Application.Jerseys;

public interface IJerseyService
{
    Task<List<JerseyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<JerseyDto?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<JerseyDto> CreateJerseyAsync(CreateJerseyDto dto, CancellationToken cancellationToken = default);

}