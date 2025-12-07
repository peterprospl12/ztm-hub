using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZtmHub.Domain.Entities;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Infrastructure.Persistence.Configurations;

public class UserStopConfiguration : IEntityTypeConfiguration<UserStop>
{
    public void Configure(EntityTypeBuilder<UserStop> builder)
    {
        builder.HasKey(us => us.Id);

        builder.Property(us => us.StopId)
            .HasConversion(stopId => stopId.Value,
                value => new StopId(value));

        builder.Property(us => us.DisplayName)
            .HasMaxLength(200);

        builder.HasIndex(us => new { us.UserId, us.StopId })
            .IsUnique();
    }
}