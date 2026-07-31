using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;
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
    .ThenInclude(f => f.FoodCategory)
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
    .ThenInclude(f => f.FoodCategory)
            .FirstOrDefaultAsync(
                x => x.HealthProfileId == healthProfileId &&
                     x.FoodId == foodId,
                cancellationToken);
    }



 
    public async Task AddAsync(
    FoodPreference preference,
    CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(preference, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        FoodPreference preference,
        CancellationToken cancellationToken = default)
    {
        _dbSet.Update(preference);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        FoodPreference preference,
        CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(preference);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<FoodPreference>> GetAllergiesAsync(int healthProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.FoodPreferences
    .Where(x =>
        x.HealthProfileId == healthProfileId &&
        x.PreferenceType == FoodPreferenceType.Allergy)
    .ToListAsync(cancellationToken);
    }
}