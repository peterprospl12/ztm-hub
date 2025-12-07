using FluentResults;
using MediatR;
using ZtmHub.Application.DTOs;
using ZtmHub.Application.Interfaces;
using ZtmHub.Application.Repositories;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Application.UseCases.Auth;

public class LoginHandler(IJwtService jwtService, IUserRepository userRepository, IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand command, CancellationToken ct)
    {
        var email = new Email(command.Email);

        var user = await userRepository.GetByEmailAsync(email, ct);
        if (user == null)
            return Result.Fail<LoginResult>("User with such email does not exist");

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Fail<LoginResult>("Password does not match");

        var token = jwtService.GenerateToken(user);

        return Result.Ok(new LoginResult(token));
    }
}