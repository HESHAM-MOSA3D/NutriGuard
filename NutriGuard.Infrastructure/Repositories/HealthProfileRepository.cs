using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;

namespace NutriGuard.Infrastructure.Repositories;

public class HealthProfileRepository
    : GenericRepository<HealthProfile>, IHealthProfileRepository
{
    public HealthProfileRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<HealthProfile?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);
    }
}