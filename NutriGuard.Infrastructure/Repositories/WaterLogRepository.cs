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

public class WaterLogRepository : GenericRepository<WaterLog>, IWaterLogRepository
{
    public WaterLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WaterLog>> GetUserWaterLogsByDateAsync(
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.UserId == userId && x.Date == date)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<double> GetTotalWaterIntakeByDateAsync(
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.UserId == userId && x.Date == date)
            .SumAsync(x => x.AmountInMl, cancellationToken);
    }
}
