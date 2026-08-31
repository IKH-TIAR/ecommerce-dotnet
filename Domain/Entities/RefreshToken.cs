namespace Ecommerce.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Token { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }

    // Computed domain state properties
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
