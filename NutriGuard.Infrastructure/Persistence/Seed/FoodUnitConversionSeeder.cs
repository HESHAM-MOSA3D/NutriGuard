using Microsoft.EntityFrameworkCore;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Infrastructure.Persistence.Seed;

public class FoodUnitConversionSeeder
{
    private readonly AppDbContext _context;

    public FoodUnitConversionSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(string csvPath)
    {
        if (await _context.FoodUnitConversions.AnyAsync())
            return;

        var lines = await File.ReadAllLinesAsync(csvPath);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = line.Split(',');

            var entity = new FoodUnitConversion
            {
                FoodId = int.Parse(values[0]),
                Unit = Enum.Parse<Unit>(values[1]),
                GramsPerUnit = double.Parse(values[2]),
                IsDefault = bool.Parse(values[3])
            };

            _context.FoodUnitConversions.Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}