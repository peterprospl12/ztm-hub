using FluentResults;
using MediatR;
using ZtmHub.Application.Interfaces;

namespace ZtmHub.Application.UseCases.Stops;

public class DeleteUserStopHandler(IUserStopRepository userStopRepository)
    : IRequestHandler<DeleteUserStopCommand, Result>
{
    public async Task<Result> Handle(DeleteUserStopCommand command, CancellationToken ct)
    {
        var userStop = await userStopRepository.GetByUserIdAndStopIdAsync(command.UserId, command.UserStopId, ct);

        if (userStop is null)
            return Result.Fail("User stop not found");

        await userStopRepository.DeleteAsync(command.UserStopId, ct);
        return Result.Ok();
    }
}