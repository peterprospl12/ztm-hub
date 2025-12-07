using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;
using ZtmHub.Application.Interfaces;

namespace ZtmHub.Application.UseCases.Stops;

public class GetDeparturesHandler(IZtmService ztmService)
    : IRequestHandler<GetDeparturesQuery, Result<StopDeparturesDto>>
{
    public async Task<Result<StopDeparturesDto>> Handle(GetDeparturesQuery query, CancellationToken ct)
    {
        var departures = await ztmService.GetDeparturesAsync(query.StopId, ct);

        if (departures == null)
            return Result.Fail<StopDeparturesDto>("Stop not found or no departures available");

        return Result.Ok(departures);
    }
}