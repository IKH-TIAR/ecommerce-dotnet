using Ecommerce.Application.Jerseys.Dtos;

namespace Ecommerce.Application.Jerseys;

public interface IJerseyService
{
    Task<List<JerseyDto>> GetAllAsync(CancellationToken cancellationToken = default);

}