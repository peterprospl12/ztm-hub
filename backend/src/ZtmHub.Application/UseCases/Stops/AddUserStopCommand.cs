using FluentResults;
using MediatR;

namespace ZtmHub.Application.UseCases.Stops;

public record AddUserStopCommand(Guid UserId, int StopId, string? CustomName = null) : IRequest<Result<Guid>>;