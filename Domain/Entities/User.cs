using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class User
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public required string FullName {get; set;}
    public required string Email {get; set;}
    public required string PasswordHash {get; set;}
    public UserRole Role {get; set;} = UserRole.Customer;

    public DateTimeOffset CreatedAt {get; set;} = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt {get; set;}

    // Navigation property: One User has Many RefreshTokens (e.g. across multiple devices)
    public List<RefreshToken> RefreshTokens { get; set; } = [];

    // Navigation property: One User has Many Orders
    public List<Order> Orders { get; set; } = [];
}