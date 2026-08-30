using Ecommerce.Application.Auth;
using Ecommerce.Application.Auth.Dtos;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Services;

public class AuthService(AppDbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // 1. Fetch user by email                                                                                                                                                      
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null)
        {
            throw new BadHttpRequestException("Invalid email or password.");
        }
                                                                                                                                                                 
        var isPasswordValid = passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new BadHttpRequestException("Invalid email or password.");
        }
                                                                                                                                              
        var token = jwtTokenGenerator.GenerateToken(user);

        var userDto = new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedAt,
            user.UpdatedAt
        );

        return new LoginResponseDto(token, userDto);
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

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
            null
        );
    }
}