using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IFoodUnitConversionRepository
{
    Task<FoodUnitConversion?> GetConversionAsync(
        int foodId,
        Unit unit,
        CancellationToken cancellationToken = default);

    Task<List<FoodUnitConversion>> GetByFoodIdsAsync(
    IEnumerable<int> foodIds,
    CancellationToken cancellationToken = default);
}