using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZtmHub.Application.UseCases.Stops;

namespace ZtmHub.Api.Controllers;

[Authorize]
public class StopsController(ISender sender) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> AddStop([FromBody] AddUserStopRequest request, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var command = new AddUserStopCommand(userId, request.StopId, request.CustomName);
        var result = await sender.Send(command, ct);

        return HandleResult(result);
    }

    [HttpDelete("{userStopId:guid}")]
    public async Task<IActionResult> DeleteStop(Guid userStopId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var command = new DeleteUserStopCommand(userStopId, userId);
        var result = await sender.Send(command, ct);

        return HandleResult(result);
    }

    [HttpPut("{userStopId:guid}")]
    public async Task<IActionResult> UpdateStop(Guid userStopId, [FromBody] UpdateUserStopRequest request,
        CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var command = new UpdateUserStopCommand(userId, userStopId, request.DisplayName);
        var result = await sender.Send(command, ct);

        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserStops(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var query = new GetUserStopsQuery(userId);
        var result = await sender.Send(query, ct);

        return HandleResult(result);
    }


    [HttpGet("{stopId:int}/departures")]
    public async Task<IActionResult> GetDepartures(int stopId, CancellationToken ct)
    {
        var query = new GetDeparturesQuery(stopId);
        var result = await sender.Send(query, ct);

        return HandleResult(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllStops(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        var query = new GetAllStopsQuery(userId);
        var result = await sender.Send(query, ct);

        return HandleResult(result);
    }

    private Guid GetUserIdFromToken()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdString, out var userId)) return userId;

        throw new UnauthorizedAccessException("Invalid User ID in token");
    }
}

public record AddUserStopRequest(int StopId, string? CustomName);

public record UpdateUserStopRequest(string? DisplayName);