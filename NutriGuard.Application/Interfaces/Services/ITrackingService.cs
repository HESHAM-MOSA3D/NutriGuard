using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NutriGuard.Application.DTOs.Tracking;

namespace NutriGuard.Application.Interfaces.Services;

public interface ITrackingService
{
    Task<MealLogResponseDto> LogMealAsync(string userId, LogMealRequestDto request, CancellationToken cancellationToken = default);
    Task<WaterLogResponseDto> LogWaterAsync(string userId, LogWaterRequestDto request, CancellationToken cancellationToken = default);
    Task<WeightLogResponseDto> LogWeightAsync(string userId, LogWeightRequestDto request, CancellationToken cancellationToken = default);
    Task<DailySummaryDto> GetDailySummaryAsync(string userId, DateOnly date, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailySummaryDto>> GetDailyHistoryAsync(string userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<BaseResponse> DeleteMealAsync(string userId, int mealLogId, CancellationToken cancellationToken = default);
    Task<BaseResponse> DeleteWaterAsync(string userId, int waterLogId, CancellationToken cancellationToken = default);
    Task<BaseResponse> DeleteWeightAsync(string userId, int weightLogId, CancellationToken cancellationToken = default);
}
