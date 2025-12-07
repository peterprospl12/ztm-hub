using System.Text.Json;
using ZtmHub.Domain.Exceptions;

namespace ZtmHub.Api.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "There was an error during processing the request: {Message}", e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, error) = exception switch
        {
            InvalidEmailException => (StatusCodes.Status400BadRequest, "Validation Error"),
            InvalidPasswordHashException => (StatusCodes.Status400BadRequest, "Validation Error"),
            InvalidDisplayNameException => (StatusCodes.Status400BadRequest, "Validation Error"),
            InvalidStopIdException => (StatusCodes.Status400BadRequest, "Validation Error"),

            StopNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),

            StopAlreadyAddedException => (StatusCodes.Status409Conflict, "Conflict"),

            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var response = new
        {
            error,
            message = exception is DomainException
                ? exception.Message
                : "Internal server error."
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}