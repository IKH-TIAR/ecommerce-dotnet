using Ecommerce.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();
    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hashPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, hashPassword, password);
        return result != PasswordVerificationResult.Failed;
    }
}