using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NutriGuard.Infrastructure.Repositories;

public class MealLogRepository : GenericRepository<MealLog>, IMealLogRepository
{
    public MealLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MealLog>> GetUserMealLogsByDateAsync(
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.MealItems)
                .ThenInclude(i => i.Food)
            .Where(x => x.UserId == userId && x.Date == date)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MealLog?> GetMealLogByIdWithItemsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.MealItems)
                .ThenInclude(i => i.Food)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MealLog>> GetUserMealLogsInDateRangeAsync(
        string userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.MealItems)
                .ThenInclude(i => i.Food)
            .Where(x => x.UserId == userId && x.Date >= startDate && x.Date <= endDate)
            .OrderBy(x => x.Date)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
