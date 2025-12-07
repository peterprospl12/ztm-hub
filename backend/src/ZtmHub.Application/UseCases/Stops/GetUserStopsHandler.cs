using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;
using ZtmHub.Application.Interfaces;

namespace ZtmHub.Application.UseCases.Stops;

public class GetUserStopsHandler(IUserStopRepository userStopRepository)
    : IRequestHandler<GetUserStopsQuery, Result<IEnumerable<UserStopDto>>>
{
    public async Task<Result<IEnumerable<UserStopDto>>> Handle(GetUserStopsQuery query, CancellationToken ct)
    {
        var userStops = await userStopRepository.GetAllByUserIdAsync(query.UserId, ct);

        var userStopsDto = userStops.Select(stop => new UserStopDto(
            stop.Id,
            stop.StopId.Value,
            stop.DisplayName,
            stop.AddedAt
        ));

        return Result.Ok(userStopsDto);
    }
}