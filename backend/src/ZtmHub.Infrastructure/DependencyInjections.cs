using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZtmHub.Application.Interfaces;
using ZtmHub.Application.Repositories;
using ZtmHub.Infrastructure.Persistence;
using ZtmHub.Infrastructure.Repositories;
using ZtmHub.Infrastructure.Services;

namespace ZtmHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ZtmHubDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserStopRepository, UserStopRepository>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddMemoryCache();
        services.AddHttpClient("ZtmClient", client => { client.Timeout = TimeSpan.FromSeconds(20); });
        services.AddScoped<IZtmService, ZtmService>();

        return services;
    }
}