using ZtmHub.Domain.Entities;

namespace ZtmHub.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}