using Microsoft.EntityFrameworkCore;
using ZtmHub.Application.Interfaces;
using ZtmHub.Domain.Entities;
using ZtmHub.Infrastructure.Persistence;

namespace ZtmHub.Infrastructure.Repositories;

public class UserStopRepository(ZtmHubDbContext context) : IUserStopRepository
{
    public async Task<IEnumerable<UserStop>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.UserStops
            .Where(us => us.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<UserStop?> GetByUserIdAndStopIdAsync(Guid userId, Guid userStopId, CancellationToken ct = default)
    {
        return await context.UserStops
            .FirstOrDefaultAsync(us => us.Id == userStopId && us.UserId == userId, ct);
    }

    public async Task AddAsync(UserStop userStop, CancellationToken ct)
    {
        await context.UserStops.AddAsync(userStop, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userStopId, CancellationToken ct)
    {
        await context.UserStops
            .Where(us => us.Id == userStopId)
            .ExecuteDeleteAsync(ct);
    }

    public async Task UpdateAsync(UserStop userStop, CancellationToken ct)
    {
        context.UserStops.Update(userStop);
        await context.SaveChangesAsync(ct);
    }
}