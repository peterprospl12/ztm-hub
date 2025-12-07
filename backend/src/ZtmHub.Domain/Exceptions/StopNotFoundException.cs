using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Domain.Exceptions;

public class StopNotFoundException(StopId stopId) : DomainException("Stop {stopId.value} not found");