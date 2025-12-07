using ZtmHub.Domain.Exceptions;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Domain.Entities;

public class UserStop
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public StopId StopId { get; private set; }
    public string DisplayName { get; private set; }
    public DateTime AddedAt { get; private set; }
    
    private UserStop() {}

    public UserStop(Guid userId, StopId stopId, string displayName)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        StopId = stopId;
        DisplayName = displayName;
        AddedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new InvalidDisplayNameException(StopId, newName);
        DisplayName = newName;
    }
}