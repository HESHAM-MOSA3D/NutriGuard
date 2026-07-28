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

public class WeightLogRepository : GenericRepository<WeightLog>, IWeightLogRepository
{
    public WeightLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<WeightLog?> GetLatestUserWeightLogAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeightLog>> GetUserWeightLogsRangeAsync(
        string userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.UserId == userId && x.Date >= startDate && x.Date <= endDate)
            .OrderBy(x => x.Date)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
