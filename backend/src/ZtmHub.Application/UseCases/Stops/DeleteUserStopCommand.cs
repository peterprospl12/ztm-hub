using FluentResults;
using MediatR;

namespace ZtmHub.Application.UseCases.Stops;

public record DeleteUserStopCommand(Guid UserStopId, Guid UserId) : IRequest<Result>;