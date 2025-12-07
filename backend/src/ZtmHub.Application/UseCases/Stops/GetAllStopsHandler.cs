using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;
using ZtmHub.Application.Interfaces;
using ZtmHub.Application.Repositories;

namespace ZtmHub.Application.UseCases.Stops;

public class GetAllStopsHandler(IUserRepository userRepository, IZtmService ztmService)
    : IRequestHandler<GetAllStopsQuery, Result<IEnumerable<StopDto>>>
{
    public async Task<Result<IEnumerable<StopDto>>> Handle(GetAllStopsQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, ct);

        if (user is null)
            return Result.Fail("Access Denied. Invalid user");

        var allStops = await ztmService.GetAllStopsAsync(ct);
        return Result.Ok(allStops);
    }
}