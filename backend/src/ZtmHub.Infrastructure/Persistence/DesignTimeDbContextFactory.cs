using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZtmHub.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ZtmHubDbContext>
{
    public ZtmHubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ZtmHubDbContext>();
        optionsBuilder.UseSqlite("Data Source=ztmhub.db");

        return new ZtmHubDbContext(optionsBuilder.Options);
    }
}