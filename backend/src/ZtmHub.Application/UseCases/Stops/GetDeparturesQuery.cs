using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;

namespace ZtmHub.Application.UseCases.Stops;

public record GetDeparturesQuery(int StopId) : IRequest<Result<StopDeparturesDto>>;