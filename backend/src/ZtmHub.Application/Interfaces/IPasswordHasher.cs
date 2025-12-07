using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Application.Interfaces;

public interface IPasswordHasher
{
    PasswordHash Hash(string password);
    bool Verify(string password, PasswordHash hash);
}