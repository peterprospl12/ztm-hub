using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;

namespace ZtmHub.Application.UseCases.Auth;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResult>>;
