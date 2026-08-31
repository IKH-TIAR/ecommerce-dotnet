using System.Security.Cryptography;
using Ecommerce.Application.Auth;
using Ecommerce.Application.Auth.Dtos;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Services;

public class AuthService(
    AppDbContext dbContext, 
    IPasswordHasher passwordHasher, 
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    private const int RefreshTokenLifetimeDays = 7;

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new BadHttpRequestException($"A user with the email '{dto.Email}' is already registered");
        }

        var hashPassword = passwordHasher.HashPassword(dto.Password);

        var user = new User
        {
            FullName = dto.FullName,
            Email = normalizedEmail,
            PasswordHash = hashPassword,
            Role = UserRole.Customer
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedAt,
            user.UpdatedAt
        );
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null)
        {
            throw new BadHttpRequestException("Invalid email or password.");
        }

        var isPasswordValid = passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new BadHttpRequestException("Invalid email or password.");
        }

        // 1. Generate Short-Lived JWT Access Token
        var accessToken = jwtTokenGenerator.GenerateToken(user);

        // 2. Generate and Save Long-Lived Refresh Token in PostgreSQL
        var refreshToken = new RefreshToken
        {
            Token = GenerateSecureRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenLifetimeDays)
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedAt,
            user.UpdatedAt
        );

        return new LoginResponseDto(accessToken, refreshToken.Token, userDto);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // 1. Find the refresh token in PostgreSQL along with the associated User
        var existingToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        // 2. Security Check: Token must exist, not be revoked, not be expired, and have a valid user
        if (existingToken is null || !existingToken.IsActive || existingToken.User is null)
        {
            throw new BadHttpRequestException("Invalid or expired refresh token.");
        }

        // 3. REFRESH TOKEN ROTATION: Revoke the old token immediately
        existingToken.RevokedAt = DateTimeOffset.UtcNow;

        // 4. Create and save a brand new Refresh Token
        var newRefreshToken = new RefreshToken
        {
            Token = GenerateSecureRefreshToken(),
            UserId = existingToken.UserId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenLifetimeDays)
        };

        await dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        // 5. Generate a fresh new JWT Access Token
        var newAccessToken = jwtTokenGenerator.GenerateToken(existingToken.User);

        var userDto = new UserDto(
            existingToken.User.Id,
            existingToken.User.FullName,
            existingToken.User.Email,
            existingToken.User.Role,
            existingToken.User.CreatedAt,
            existingToken.User.UpdatedAt
        );

        return new LoginResponseDto(newAccessToken, newRefreshToken.Token, userDto);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existingToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (existingToken is null || existingToken.IsRevoked)
        {
            return false;
        }

        // Revoke the token (Logout)
        existingToken.RevokedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string GenerateSecureRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}