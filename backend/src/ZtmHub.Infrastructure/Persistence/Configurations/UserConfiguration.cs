using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZtmHub.Domain.Entities;
using ZtmHub.Domain.ValueObjects;

namespace ZtmHub.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasConversion(
                email => email.Value,
                value => new Email(value));

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasConversion(
                passwordHash => passwordHash.Value,
                value => new PasswordHash(value));

        builder.HasMany(u => u.Stops)
            .WithOne()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}