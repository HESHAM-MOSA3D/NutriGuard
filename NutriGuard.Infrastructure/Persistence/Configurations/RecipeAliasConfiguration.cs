using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Configurations;

public class RecipeAliasConfiguration : IEntityTypeConfiguration<RecipeAlias>
{
    public void Configure(EntityTypeBuilder<RecipeAlias> builder)
    {
        builder.ToTable("RecipeAliases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Alias)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Language)
            .HasConversion<int>()    
            .IsRequired();

        builder.HasOne(x => x.Recipe)
            .WithMany(x => x.RecipeAliases)
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.RecipeId,
            x.Alias
        }).IsUnique();
    }
}