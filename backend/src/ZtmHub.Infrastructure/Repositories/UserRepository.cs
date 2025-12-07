using Microsoft.EntityFrameworkCore;
using ZtmHub.Application.Repositories;
using ZtmHub.Domain.Entities;
using ZtmHub.Domain.ValueObjects;
using ZtmHub.Infrastructure.Persistence;

namespace ZtmHub.Infrastructure.Repositories;

public class UserRepository(ZtmHubDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<bool> ExistsAsync(Email email, CancellationToken ct = default)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }
}