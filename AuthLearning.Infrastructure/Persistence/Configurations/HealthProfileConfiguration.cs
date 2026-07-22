using NutriGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class HealthProfileConfiguration : IEntityTypeConfiguration<HealthProfile>
{
    public void Configure(EntityTypeBuilder<HealthProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Height)
               .IsRequired();

        builder.Property(x => x.Weight)
               .IsRequired();

        builder.Property(x => x.DateOfBirth)
               .IsRequired();

        builder.Property(x => x.Gender)
               .IsRequired();

        builder.Property(x => x.ActivityLevel)
               .IsRequired();

        builder.Property(x => x.DietType)
               .IsRequired();

        builder.Property(x => x.Goal)
               .IsRequired();

        builder.HasOne(x => x.User)
               .WithOne()
               .HasForeignKey<HealthProfile>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId)
               .IsUnique();
    }
}