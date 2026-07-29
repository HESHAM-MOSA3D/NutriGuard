using NutriGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IWaterLogRepository : IGenericRepository<WaterLog>
{
    Task<IEnumerable<WaterLog>> GetUserWaterLogsByDateAsync(
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<double> GetTotalWaterIntakeByDateAsync(
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WaterLog>> GetUserWaterLogsInDateRangeAsync(
        string userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
