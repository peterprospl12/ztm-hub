using FluentResults;
using MediatR;
using ZtmHub.Application.Interfaces;

namespace ZtmHub.Application.UseCases.Stops;

public class UpdateUserStopHandler(IUserStopRepository userStopRepository)
    : IRequestHandler<UpdateUserStopCommand, Result>
{
    public async Task<Result> Handle(UpdateUserStopCommand command, CancellationToken ct)
    {
        var userStop = await userStopRepository.GetByUserIdAndStopIdAsync(command.UserId, command.StopId, ct);

        if (userStop is null)
            return Result.Fail("User stop not found");

        if (userStop.UserId != command.UserId)
            return Result.Fail("Access denied");

        userStop.UpdateDisplayName(command.DisplayName);

        await userStopRepository.UpdateAsync(userStop, ct);
        return Result.Ok();
    }
}