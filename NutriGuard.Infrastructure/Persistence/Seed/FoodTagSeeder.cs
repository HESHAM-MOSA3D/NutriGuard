using Microsoft.EntityFrameworkCore;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Seed;

public class FoodTagSeeder
{
    private readonly AppDbContext _context;

    public FoodTagSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(string csvPath)
    {
        if (await _context.FoodTags.AnyAsync())
            return;

        var lines = await File.ReadAllLinesAsync(csvPath);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = line.Split(',');

            var entity = new FoodTag
            {
                Id = int.Parse(values[0]),
                Name = values[1]
            };

            _context.FoodTags.Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}