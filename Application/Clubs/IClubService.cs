using Ecommerce.Application.Clubs.Dtos;
using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Clubs;

public interface IClubService
{
    Task<PagedResult<ClubDto>> GetAllClubAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<ClubDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClubDto> CreateClubAsync(CreateClubDto dto, CancellationToken cancellationToken = default);
    Task<ClubDto?> UpdateClubAsync(Guid id, UpdateClubDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteClubAsync(Guid id, CancellationToken cancellationToken = default);
}