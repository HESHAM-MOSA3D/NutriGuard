using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("Foods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.RefusePercentage).HasPrecision(6, 2);
        builder.Property(x => x.Water).HasPrecision(8, 2);
        builder.Property(x => x.Energy).HasPrecision(8, 2);
        builder.Property(x => x.Protein).HasPrecision(8, 2);
        builder.Property(x => x.Fat).HasPrecision(8, 2);
        builder.Property(x => x.Ash).HasPrecision(8, 2);
        builder.Property(x => x.Fiber).HasPrecision(8, 2);
        builder.Property(x => x.Carbohydrate).HasPrecision(8, 2);

        builder.Property(x => x.Sodium).HasPrecision(10, 2);
        builder.Property(x => x.Potassium).HasPrecision(10, 2);
        builder.Property(x => x.Calcium).HasPrecision(10, 2);
        builder.Property(x => x.Phosphorus).HasPrecision(10, 2);
        builder.Property(x => x.Magnesium).HasPrecision(10, 2);
        builder.Property(x => x.Iron).HasPrecision(10, 2);
        builder.Property(x => x.Zinc).HasPrecision(10, 2);
        builder.Property(x => x.Copper).HasPrecision(10, 2);

        builder.Property(x => x.VitaminA).HasPrecision(10, 2);
        builder.Property(x => x.VitaminC).HasPrecision(10, 2);
        builder.Property(x => x.Thiamin).HasPrecision(10, 2);
        builder.Property(x => x.Riboflavin).HasPrecision(10, 2);

        builder.HasIndex(new[] { nameof(Food.Name) }, "ix_foods_name_unique")
            .IsUnique();

        builder.HasIndex(new[] { nameof(Food.Name) }, "idx_foods_name_trgm")
            .IsUnique(false)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.HasIndex(x => x.FoodCategoryId)
            .HasDatabaseName("idx_foods_categoryid");

        builder.HasOne(x => x.FoodCategory)
            .WithMany(x => x.Foods)
            .HasForeignKey(x => x.FoodCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}