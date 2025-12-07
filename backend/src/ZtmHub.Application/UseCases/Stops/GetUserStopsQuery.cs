using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;

namespace ZtmHub.Application.UseCases.Stops;

public record GetUserStopsQuery(Guid UserId) : IRequest<Result<IEnumerable<UserStopDto>>>;