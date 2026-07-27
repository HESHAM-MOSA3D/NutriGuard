using NutriGuard.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NutriGuard.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
    public DbSet<HealthProfile> HealthProfiles { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<FoodAlias> FoodAliases { get; set; }
    public DbSet<FoodPreference> FoodPreferences { get; set; }
    public DbSet<FoodCategory> FoodCategories => Set<FoodCategory>();
    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("pg_trgm");

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}