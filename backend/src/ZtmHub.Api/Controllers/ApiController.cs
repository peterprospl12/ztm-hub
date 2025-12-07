using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ZtmHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(result.Value);

        return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return Ok();

        return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
    }
}