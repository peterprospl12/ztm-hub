using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Domain.Exceptions;

public class InvalidDisplayNameException(StopId stopId, string displayName) : DomainException("Invalid display name {displayName} for stop {stopId}");