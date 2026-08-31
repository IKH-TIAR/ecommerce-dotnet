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
    IJwtTokenGenerator jwtTokenGenerator,
    ILogger<AuthService> logger) : IAuthService
{
    private const int RefreshTokenLifetimeDays = 7;

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            logger.LogWarning("Registration failed: Email {Email} is already registered", dto.Email);
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

        logger.LogInformation("New user {UserId} registered successfully with email {Email}", user.Id, user.Email);

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
            logger.LogWarning("Login failed: User with email {Email} not found", dto.Email);
            throw new BadHttpRequestException("Invalid email or password.");
        }

        var isPasswordValid = passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            logger.LogWarning("Login failed: Invalid password for user {UserId} ({Email})", user.Id, user.Email);
            throw new BadHttpRequestException("Invalid email or password.");
        }

        var accessToken = jwtTokenGenerator.GenerateToken(user);

        var refreshToken = new RefreshToken
        {
            Token = GenerateSecureRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenLifetimeDays)
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} ({Email}) logged in successfully with Role {Role}", user.Id, user.Email, user.Role);

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
        var existingToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (existingToken is null || !existingToken.IsActive || existingToken.User is null)
        {
            logger.LogWarning("Token refresh failed: Invalid, revoked, or expired refresh token attempted");
            throw new BadHttpRequestException("Invalid or expired refresh token.");
        }

        // ROTATION: Revoke old token
        existingToken.RevokedAt = DateTimeOffset.UtcNow;

        var newRefreshToken = new RefreshToken
        {
            Token = GenerateSecureRefreshToken(),
            UserId = existingToken.UserId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenLifetimeDays)
        };

        await dbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var newAccessToken = jwtTokenGenerator.GenerateToken(existingToken.User);

        logger.LogInformation("Refresh token rotated successfully for User {UserId}", existingToken.UserId);

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

        existingToken.RevokedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} session successfully revoked (Logged out)", existingToken.UserId);

        return true;
    }

    private static string GenerateSecureRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}