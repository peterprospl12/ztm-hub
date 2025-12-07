using ZtmHub.Domain.Entities;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Domain.Exceptions;

public class StopAlreadyAddedException(StopId stopId) : DomainException("Stop {stopId.value} has been already added");