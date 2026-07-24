using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class FoodPreferenceConfiguration : IEntityTypeConfiguration<FoodPreference>
{
    public void Configure(EntityTypeBuilder<FoodPreference> builder)
    {
        builder.ToTable("FoodPreferences");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.HealthProfile)
            .WithMany(x => x.FoodPreferences)
            .HasForeignKey(x => x.HealthProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Food)
            .WithMany(x => x.FoodPreferences)
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.HealthProfileId,
            x.FoodId,
            x.PreferenceType
        }).IsUnique();
    }
}