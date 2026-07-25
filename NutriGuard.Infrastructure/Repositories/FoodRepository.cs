using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence.Configurations;
using NutriGuard.Infrastructure.Repositories;

namespace NutriGuard.Infrastructure.Persistence.Repositories;

public class FoodRepository
    : GenericRepository<Food>, IFoodRepository
{
    private readonly AppDbContext _context;

    public FoodRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Food>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        query = query.Trim();

        return await _context.Foods
            .AsNoTracking()
            .Where(f =>
                EF.Functions.ILike(f.Name, $"%{query}%") ||

                f.Aliases.Any(a =>
                    EF.Functions.ILike(a.Alias, $"%{query}%")))
            .OrderBy(f => f.Name)
            .ToListAsync(cancellationToken);
    }
}