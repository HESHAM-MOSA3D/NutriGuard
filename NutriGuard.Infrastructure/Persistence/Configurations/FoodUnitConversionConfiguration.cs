using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class FoodUnitConversionConfiguration
    : IEntityTypeConfiguration<FoodUnitConversion>
{
    public void Configure(EntityTypeBuilder<FoodUnitConversion> builder)
    {
        builder.ToTable("FoodUnitConversions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GramsPerUnit)
            .IsRequired();

        builder.Property(x => x.Unit)
            .IsRequired();

        builder.Property(x => x.IsDefault)
            .IsRequired();

        builder.HasOne(x => x.Food)
            .WithMany(x => x.UnitConversions)
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.FoodId, x.Unit })
            .IsUnique();
    }
}