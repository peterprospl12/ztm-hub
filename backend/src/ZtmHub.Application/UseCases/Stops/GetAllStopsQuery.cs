using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;

namespace ZtmHub.Application.UseCases.Stops;

public record GetAllStopsQuery(Guid UserId) : IRequest<Result<IEnumerable<StopDto>>>;