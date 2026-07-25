using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;

namespace NutriGuard.Infrastructure.Repositories;

public sealed class FoodPreferenceRepository
    : GenericRepository<FoodPreference>, IFoodPreferenceRepository
{
    public FoodPreferenceRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<FoodPreference>> GetByHealthProfileIdAsync(
        int healthProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.HealthProfileId == healthProfileId)
            .Include(x => x.Food)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int healthProfileId,
        int foodId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.HealthProfileId == healthProfileId &&
                 x.FoodId == foodId,
            cancellationToken);
    }


    public async Task<FoodPreference?> GetAsync(
    int healthProfileId,
    int foodId,
    CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Food)
            .FirstOrDefaultAsync(
                x => x.HealthProfileId == healthProfileId &&
                     x.FoodId == foodId,
                cancellationToken);
    }
}