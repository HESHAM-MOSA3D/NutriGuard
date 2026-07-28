using NutriGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IWeightLogRepository : IGenericRepository<WeightLog>
{
    Task<WeightLog?> GetLatestUserWeightLogAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WeightLog>> GetUserWeightLogsRangeAsync(
        string userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
