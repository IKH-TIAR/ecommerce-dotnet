using Ecommerce.Application.Auth.Dtos;

namespace Ecommerce.Application.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}