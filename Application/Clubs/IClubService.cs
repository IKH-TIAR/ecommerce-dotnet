using Ecommerce.Application.Clubs.Dtos;
using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Clubs;
public interface IClubService
{
    Task<ClubDto> CreateClubAsync (CreateClubDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<ClubDto>> GetAllClubAsync (Guid id, CancellationToken cancellationToken = default);
}