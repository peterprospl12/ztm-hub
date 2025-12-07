using FluentResults;
using MediatR;

namespace ZtmHub.Application.UseCases.Stops;

public record UpdateUserStopCommand(Guid UserId, Guid StopId, string DisplayName) : IRequest<Result>;