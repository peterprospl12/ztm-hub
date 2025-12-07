using FluentResults;
using MediatR;
using ZtmHub.Application.Interfaces;
using ZtmHub.Application.Repositories;
using ZtmHub.Domain.Entities;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Application.UseCases.Auth;

public class RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var email = new Email(command.Email);

        if (await userRepository.ExistsAsync(email, ct))
            return Result.Fail<Guid>("Email already exists");

        var hash = passwordHasher.Hash(command.Password);
        var user = new User(email, hash);

        await userRepository.AddAsync(user, ct);
        return Result.Ok(user.Id);
    }
}