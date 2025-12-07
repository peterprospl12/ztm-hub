using ZtmHub.Domain.Entities;

namespace ZtmHub.Application.Interfaces;

public interface IUserStopRepository
{
    Task<IEnumerable<UserStop>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task<UserStop?> GetByUserIdAndStopIdAsync(Guid userId, Guid userStopId, CancellationToken ct = default);

    Task AddAsync(UserStop userStop, CancellationToken ct);

    Task DeleteAsync(Guid userStopId, CancellationToken ct);

    Task UpdateAsync(UserStop userStop, CancellationToken ct);
}