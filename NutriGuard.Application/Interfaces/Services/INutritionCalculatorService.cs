using NutriGuard.Application.DTOs.Nutrition;

namespace NutriGuard.Application.Interfaces.Services;

public interface INutritionCalculatorService
{
    Task<NutritionTargetResponseDto> CalculateAsync(
        string userId,
        CancellationToken cancellationToken = default);
}