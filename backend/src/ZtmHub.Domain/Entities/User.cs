using ZtmHub.Domain.Exceptions;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Email Email  { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<UserStop> _stops = new();
    public IReadOnlyCollection<UserStop> Stops => _stops.AsReadOnly();

    private User() {}

    public User(Email email, PasswordHash passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddStop(StopId stopId, string displayName)
    {
        if (_stops.Any(s => s.StopId == stopId))
            throw new StopAlreadyAddedException(stopId);
        _stops.Add(new UserStop(Id, stopId, displayName));
    }

    public void RemoveStop(StopId stopId)
    {
        var stop = _stops.FirstOrDefault(s => s.StopId == stopId);
        if  (stop != null)
            throw new StopNotFoundException(stopId);
        _stops.Remove(stop);
    }
}
