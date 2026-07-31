using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class FoodTagAssignmentConfiguration : IEntityTypeConfiguration<FoodTagAssignment>
{
    public void Configure(EntityTypeBuilder<FoodTagAssignment> builder)
    {
        builder.HasKey(x => new { x.FoodId, x.FoodTagId });

        builder.HasOne(x => x.Food)
               .WithMany(x => x.FoodTagAssignments)
               .HasForeignKey(x => x.FoodId);

        builder.HasOne(x => x.FoodTag)
               .WithMany(x => x.FoodTagAssignments)
               .HasForeignKey(x => x.FoodTagId);
    }
}