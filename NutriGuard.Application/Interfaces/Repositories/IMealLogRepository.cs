using NutriGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IMealLogRepository : IGenericRepository<MealLog>
{
    Task<IEnumerable<MealLog>> GetUserMealLogsByDateAsync(
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<MealLog?> GetMealLogByIdWithItemsAsync(
        int id,
        CancellationToken cancellationToken = default);
}
