using Microsoft.EntityFrameworkCore;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Infrastructure.Persistence.Seed;

public class FoodTagAssignmentSeeder
{
    private readonly AppDbContext _context;

    public FoodTagAssignmentSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(string csvPath)
    {
        if (await _context.FoodTagAssignments.AnyAsync())
            return;

        var lines = await File.ReadAllLinesAsync(csvPath);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = line.Split(',');

            var entity = new FoodTagAssignment
            {
                FoodId = int.Parse(values[0]),
                FoodTagId = int.Parse(values[1])
            };

            _context.FoodTagAssignments.Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}