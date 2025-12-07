using FluentResults;
using MediatR;
using ZtmHub.Application.Interfaces;
using ZtmHub.Application.Repositories;
using ZtmHub.Domain.Entities;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Application.UseCases.Stops;

public class AddUserStopHandler(
    IUserStopRepository userStopRepository,
    IUserRepository userRepository,
    IZtmService ztmService) :
    IRequestHandler<AddUserStopCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddUserStopCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result.Fail<Guid>("User not found");

        var stopId = new StopId(command.StopId);

        var existingStops = await userStopRepository.GetAllByUserIdAsync(command.UserId, ct);
        if (existingStops.Any(s => s.StopId.Value == command.StopId))
            return Result.Fail<Guid>("User already has this stop saved");

        var stopInfo = await ztmService.GetStopInfoAsync(command.StopId, ct);
        if (stopInfo is null)
            return Result.Fail<Guid>("Stop not found");

        var userStop = new UserStop(command.UserId, stopId, command.CustomName ?? stopInfo.Name);

        await userStopRepository.AddAsync(userStop, ct);

        return Result.Ok(userStop.Id);
    }
}