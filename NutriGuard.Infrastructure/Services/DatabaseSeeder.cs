using Microsoft.Extensions.DependencyInjection;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Infrastructure.Persistence;
using NutriGuard.Infrastructure.Persistence.Seed;

namespace NutriGuard.Infrastructure.Services;

public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();

        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();

        await FoodCategorySeeder.SeedAsync(context);

        var foodImporter =
            services.GetRequiredService<IFoodImportService>();

        await foodImporter.SeedFoodsAsync();

        var recipeImporter =
            services.GetRequiredService<IRecipeImportService>();

        await recipeImporter.SeedRecipesAsync();

    


        var foodUnitSeeder =
            services.GetRequiredService<FoodUnitConversionSeeder>();

        await foodUnitSeeder.SeedAsync(
            Path.Combine(
                AppContext.BaseDirectory,
                "SeedData",
                "FoodUnitConversions.csv"));

    


        var foodTagSeeder =
            services.GetRequiredService<FoodTagSeeder>();

        await foodTagSeeder.SeedAsync(
            Path.Combine(
                AppContext.BaseDirectory,
                "SeedData",
                "FoodTags.csv"));



        var assignmentSeeder =
            services.GetRequiredService<FoodTagAssignmentSeeder>();

        await assignmentSeeder.SeedAsync(
            Path.Combine(
                AppContext.BaseDirectory,
                "SeedData",
                "FoodTagAssignments.csv"));
    }
}