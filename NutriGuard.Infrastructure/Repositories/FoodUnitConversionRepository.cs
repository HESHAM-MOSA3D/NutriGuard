using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;
using NutriGuard.Infrastructure.Persistence;

namespace NutriGuard.Infrastructure.Repositories;

public class FoodUnitConversionRepository : IFoodUnitConversionRepository
{
    private readonly AppDbContext _context;

    public FoodUnitConversionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FoodUnitConversion?> GetConversionAsync(
        int foodId,
        Unit unit,
        CancellationToken cancellationToken = default)
    {
        return await _context.FoodUnitConversions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.FoodId == foodId &&
                     x.Unit == unit,
                cancellationToken);
    }

    public async Task<List<FoodUnitConversion>> GetByFoodIdsAsync(
    IEnumerable<int> foodIds,
    CancellationToken cancellationToken = default)
    {
        return await _context.FoodUnitConversions
            .Where(x => foodIds.Contains(x.FoodId))
            .ToListAsync(cancellationToken);
    }
}