using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;
using NutriGuard.Infrastructure.Csv;
using NutriGuard.Infrastructure.Persistence;
using System.Globalization;

namespace NutriGuard.Infrastructure.Services;

public class RecipeImportService : IRecipeImportService
{
    private readonly AppDbContext _context;
    private readonly IHostEnvironment _environment;

    public RecipeImportService(
        AppDbContext context,
        IHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task SeedRecipesAsync(
        CancellationToken cancellationToken = default)
    {
       
        if (!await _context.Recipes.AnyAsync(cancellationToken))
        {
            var recipesPath = Path.Combine(
                _environment.ContentRootPath,
                "SeedData",
                "Recipes.csv");

            using var reader = new StreamReader(recipesPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var recipeRecords = csv.GetRecords<RecipeCsvRecord>().ToList();

            var recipes = recipeRecords.Select(x => new Recipe
            {
                Name = x.Name,
                Description = x.Description,
                Instructions = x.Instructions,
                Servings = x.Servings,
                PreparationTimeMinutes = x.PreparationTimeMinutes,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Recipes.AddRangeAsync(
                recipes,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }


        var recipesDictionary = await _context.Recipes
            .ToDictionaryAsync(
                x => x.Name,
                x => x.Id,
                cancellationToken);

        var foodsDictionary = await _context.Foods
            .ToDictionaryAsync(
                x => x.Name,
                x => x.Id,
                cancellationToken);

        
        if (!await _context.RecipeIngredients.AnyAsync(cancellationToken))
        {
            var ingredientsPath = Path.Combine(
                _environment.ContentRootPath,
                "SeedData",
                "RecipeIngredients.csv");

            using var ingredientReader = new StreamReader(ingredientsPath);
            using var ingredientCsv = new CsvReader(
                ingredientReader,
                CultureInfo.InvariantCulture);

            var ingredientRecords =
                ingredientCsv.GetRecords<RecipeIngredientCsvRecord>()
                             .ToList();

            var ingredients = ingredientRecords
                .Where(x =>
                    recipesDictionary.ContainsKey(x.RecipeName) &&
                    foodsDictionary.ContainsKey(x.FoodName))
                .Select(x => new RecipeIngredient
                {
                    RecipeId = recipesDictionary[x.RecipeName],
                    FoodId = foodsDictionary[x.FoodName],
                    Quantity = x.Quantity,
                    Unit = ParseUnit(x.Unit)
                })
                .ToList();

            await _context.RecipeIngredients.AddRangeAsync(
                ingredients,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

       
        if (!await _context.RecipeAliases.AnyAsync(cancellationToken))
        {
            var aliasesPath = Path.Combine(
                _environment.ContentRootPath,
                "SeedData",
                "RecipeAliases.csv");

            using var aliasReader = new StreamReader(aliasesPath);
            using var aliasCsv = new CsvReader(
                aliasReader,
                CultureInfo.InvariantCulture);

            var aliasRecords =
                aliasCsv.GetRecords<RecipeAliasCsvRecord>()
                        .ToList();

            var aliases = aliasRecords
                .Where(x => recipesDictionary.ContainsKey(x.RecipeName))
                .Select(x => new RecipeAlias
                {
                    RecipeId = recipesDictionary[x.RecipeName],
                    Alias = x.Alias,
                    Language = (AliasLanguage)x.Language
                })
                .ToList();

            await _context.RecipeAliases.AddRangeAsync(
                aliases,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static Unit ParseUnit(string unit)
    {
        return unit.Trim().ToLower() switch
        {
            "g" => Unit.Gram,
            "ml" => Unit.Milliliter,
            "tbsp" => Unit.Tablespoon,
            "tsp" => Unit.Teaspoon,
            "piece" => Unit.Piece,
            "cup" => Unit.Cup,

            _ => throw new InvalidOperationException(
                $"Unknown unit '{unit}'.")
        };
    }
}