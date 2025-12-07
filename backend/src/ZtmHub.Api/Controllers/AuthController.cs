using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZtmHub.Application.UseCases.Auth;

namespace ZtmHub.Api.Controllers;

public class AuthController(ISender sender) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
}