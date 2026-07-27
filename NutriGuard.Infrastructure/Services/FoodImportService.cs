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

public class FoodImportService : IFoodImportService
{
    private readonly AppDbContext _context;
    private readonly IHostEnvironment _environment;

    public FoodImportService(
        AppDbContext context,
        IHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task SeedFoodsAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Food Import Started");
        if (await _context.Foods.AnyAsync(cancellationToken))
            return;

        Console.WriteLine($"Foods Count Before Import: {await _context.Foods.CountAsync(cancellationToken)}");
        var foodsPath = Path.Combine(
            _environment.ContentRootPath,
            "SeedData",
            "Foods.csv");

        using var reader = new StreamReader(foodsPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var foodRecords = csv.GetRecords<FoodCsvRecord>().ToList();


        var categoriesDictionary = await _context.FoodCategories
    .ToDictionaryAsync(
        x => x.Name,
        x => x.Id,
        cancellationToken);


        Console.WriteLine($"Food Records Read: {foodRecords.Count}");

        var foods = foodRecords.Select(x => new Food
        {
            FoodCategoryId = categoriesDictionary[x.Category],

            Name = x.Name,

            RefusePercentage = x.RefusePercentage ?? 0,
            Water = x.Water ?? 0,
            Energy = x.Energy ?? 0,
            Protein = x.Protein ?? 0,
            Fat = x.Fat ?? 0,
            Ash = x.Ash ?? 0,
            Fiber = x.Fiber ?? 0,
            Carbohydrate = x.Carbohydrate ?? 0,

            Sodium = x.Sodium ?? 0,
            Potassium = x.Potassium ?? 0,
            Calcium = x.Calcium ?? 0,
            Phosphorus = x.Phosphorus ?? 0,
            Magnesium = x.Magnesium ?? 0,
            Iron = x.Iron ?? 0,
            Zinc = x.Zinc ?? 0,
            Copper = x.Copper ?? 0,

            VitaminA = x.VitaminA,
            VitaminC = x.VitaminC ?? 0,
            Thiamin = x.Thiamin ?? 0,
            Riboflavin = x.Riboflavin ?? 0
        }).ToList();

        Console.WriteLine($"Mapped Foods: {foods.Count}");

        await _context.Foods.AddRangeAsync(foods, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Foods Saved: {await _context.Foods.CountAsync(cancellationToken)}");

        var aliasesPath = Path.Combine(
            _environment.ContentRootPath,
            "SeedData",
            "FoodAliases.csv");

        using var aliasReader = new StreamReader(aliasesPath);
        using var aliasCsv = new CsvReader(aliasReader, CultureInfo.InvariantCulture);

        var aliasRecords = aliasCsv.GetRecords<FoodAliasCsvRecord>().ToList();
        Console.WriteLine($"Alias Records Read: {aliasRecords.Count}");

        var foodsDictionary = await _context.Foods
            .ToDictionaryAsync(x => x.Name, x => x.Id, cancellationToken);

        var aliases = aliasRecords
            .Where(x => foodsDictionary.ContainsKey(x.Food))
            .Select(x => new FoodAlias
            {
                FoodId = foodsDictionary[x.Food],
                Alias = x.Alias,
                Language = (AliasLanguage)x.Language
            })
            .ToList();

        Console.WriteLine($"Mapped Aliases: {aliases.Count} (of {aliasRecords.Count} read)");

        await _context.FoodAliases.AddRangeAsync(aliases, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Aliases Saved: {await _context.FoodAliases.CountAsync(cancellationToken)}");
    }
}