using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Auth.Dtos;

public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);