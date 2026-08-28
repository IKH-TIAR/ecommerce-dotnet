using Ecommerce.Application.Clubs.Dtos;
using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Clubs;
public interface IClubService
{
    Task<ClubDto> CreateClubAsync (CreateClubDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<ClubDto>> GetAllClubAsync (int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}