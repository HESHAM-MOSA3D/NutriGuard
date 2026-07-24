using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class FoodAliasConfiguration : IEntityTypeConfiguration<FoodAlias>
{
    public void Configure(EntityTypeBuilder<FoodAlias> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Alias)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Language)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.Alias,
            x.Language
        });

        builder.HasIndex(x => new
        {
            x.FoodId,
            x.Alias,
            x.Language
        }).IsUnique();

        builder.HasOne(x => x.Food)
            .WithMany(x => x.Aliases)
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}