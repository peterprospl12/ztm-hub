using FluentResults;
using MediatR;

namespace ZtmHub.Application.UseCases.Auth;

public record RegisterCommand(string Email, string Password) : IRequest<Result<Guid>>;
