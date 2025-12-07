using ZtmHub.Application.Interfaces;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public PasswordHash Hash(string password)
    {
        return new PasswordHash(BCrypt.Net.BCrypt.HashPassword(password));
    }

    public bool Verify(string password, PasswordHash hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash.Value);
    }
}