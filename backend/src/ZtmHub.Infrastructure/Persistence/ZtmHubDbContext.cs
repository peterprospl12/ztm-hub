using Microsoft.EntityFrameworkCore;
using ZtmHub.Domain.Entities;

namespace ZtmHub.Infrastructure.Persistence;

public class ZtmHubDbContext(DbContextOptions<ZtmHubDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserStop> UserStops => Set<UserStop>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZtmHubDbContext).Assembly);
    }
}