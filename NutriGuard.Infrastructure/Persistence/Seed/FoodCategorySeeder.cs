using Microsoft.EntityFrameworkCore;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;

namespace NutriGuard.Infrastructure.Persistence.Seed;

public static class FoodCategorySeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        CancellationToken cancellationToken = default)
    {
        if (await context.FoodCategories.AnyAsync(cancellationToken))
            return;

        var categories = new List<FoodCategory>
        {
            new() { Name = "Grains" },
            new() { Name = "Fruits" },
            new() { Name = "Vegetables" },
            new() { Name = "Legumes" },
            new() { Name = "Meat" },
            new() { Name = "Poultry" },
            new() { Name = "Seafood" },
            new() { Name = "Dairy" },
            new() { Name = "Eggs" },
            new() { Name = "Nuts & Seeds" },
            new() { Name = "Oils & Fats" },
            new() { Name = "Beverages" },
            new() { Name = "Herbs & Spices" },
            new() { Name = "Sweets & Desserts" },
            new() { Name = "Bakery" },
            new() { Name = "Snacks" },
            new() { Name = "Other" }
        };

        await context.FoodCategories.AddRangeAsync(
            categories,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}