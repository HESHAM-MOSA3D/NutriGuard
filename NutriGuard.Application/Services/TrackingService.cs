using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NutriGuard.Application.DTOs.Tracking;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.Services;

public class TrackingService : ITrackingService
{
    private readonly IMealLogRepository _mealLogRepository;
    private readonly IWaterLogRepository _waterLogRepository;
    private readonly IWeightLogRepository _weightLogRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly INutritionCalculatorService _nutritionCalculator;

    public TrackingService(
        IMealLogRepository mealLogRepository,
        IWaterLogRepository waterLogRepository,
        IWeightLogRepository weightLogRepository,
        IFoodRepository foodRepository,
        INutritionCalculatorService nutritionCalculator)
    {
        _mealLogRepository = mealLogRepository;
        _waterLogRepository = waterLogRepository;
        _weightLogRepository = weightLogRepository;
        _foodRepository = foodRepository;
        _nutritionCalculator = nutritionCalculator;
    }

    public async Task<MealLogResponseDto> LogMealAsync(string userId, LogMealRequestDto request, CancellationToken cancellationToken = default)
    {
        var mealLog = new MealLog
        {
            UserId = userId,
            MealType = request.MealType,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in request.MealItems)
        {
            var food = await _foodRepository.GetByIdAsync(item.FoodId, cancellationToken);
            if (food == null)
            {
                return MealLogResponseDto.Failure($"Food with ID {item.FoodId} not found.");
            }

            mealLog.MealItems.Add(new MealItem
            {
                FoodId = item.FoodId,
                Quantity = item.Quantity,
                Unit = item.Unit
            });
        }

        await _mealLogRepository.AddAsync(mealLog, cancellationToken);
        await _mealLogRepository.SaveChangesAsync(cancellationToken);

        // Map to Dto
        var logFromDb = await _mealLogRepository.GetMealLogByIdWithItemsAsync(mealLog.Id, cancellationToken);
        var dto = MapToMealLogDto(logFromDb!);

        return MealLogResponseDto.Success(dto);
    }

    public async Task<WaterLogResponseDto> LogWaterAsync(string userId, LogWaterRequestDto request, CancellationToken cancellationToken = default)
    {
        var waterLog = new WaterLog
        {
            UserId = userId,
            AmountInMl = request.AmountInMl,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        await _waterLogRepository.AddAsync(waterLog, cancellationToken);
        await _waterLogRepository.SaveChangesAsync(cancellationToken);

        return WaterLogResponseDto.Success(new WaterLogDto
        {
            Id = waterLog.Id,
            AmountInMl = waterLog.AmountInMl,
            Date = waterLog.Date,
            CreatedAt = waterLog.CreatedAt
        });
    }

    public async Task<WeightLogResponseDto> LogWeightAsync(string userId, LogWeightRequestDto request, CancellationToken cancellationToken = default)
    {
        var weightLog = new WeightLog
        {
            UserId = userId,
            Weight = request.Weight,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        await _weightLogRepository.AddAsync(weightLog, cancellationToken);
        await _weightLogRepository.SaveChangesAsync(cancellationToken);

        return WeightLogResponseDto.Success(new WeightLogDto
        {
            Id = weightLog.Id,
            Weight = weightLog.Weight,
            Date = weightLog.Date,
            CreatedAt = weightLog.CreatedAt
        });
    }

    public async Task<DailySummaryDto> GetDailySummaryAsync(string userId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var mealLogs = await _mealLogRepository.GetUserMealLogsByDateAsync(userId, date, cancellationToken);
        var waterTotal = await _waterLogRepository.GetTotalWaterIntakeByDateAsync(userId, date, cancellationToken);
        var targetsResult = await _nutritionCalculator.CalculateAsync(userId, cancellationToken);
        var targets = targetsResult.Data;

        var summary = new DailySummaryDto
        {
            Date = date,
            WaterConsumedMl = waterTotal
        };

        if (targets != null)
        {
            summary.CaloriesTarget = targets.DailyCalories;
            summary.ProteinTargetGrams = targets.ProteinGrams;
            summary.CarbsTargetGrams = targets.CarbsGrams;
            summary.FatTargetGrams = targets.FatGrams;

            // Standard recommendation (approx 30ml per kg of body weight or simple constant). Let's default to standard 2000ml or 0 if not calc.
            // For now sticking to simple 2000 ml default if not calculated.
            summary.WaterTargetMl = 2000;
        }

        foreach (var log in mealLogs)
        {
            var logDto = MapToMealLogDto(log);
            summary.Meals.Add(logDto);

            summary.CaloriesConsumed += logDto.TotalCalories;
            summary.ProteinConsumedGrams += logDto.TotalProtein;
            summary.CarbsConsumedGrams += logDto.TotalCarbs;
            summary.FatConsumedGrams += logDto.TotalFat;
        }

        summary.CaloriesRemaining = Math.Max(0, summary.CaloriesTarget - summary.CaloriesConsumed);
        summary.ProteinRemainingGrams = Math.Max(0, summary.ProteinTargetGrams - summary.ProteinConsumedGrams);
        summary.CarbsRemainingGrams = Math.Max(0, summary.CarbsTargetGrams - summary.CarbsConsumedGrams);
        summary.FatRemainingGrams = Math.Max(0, summary.FatTargetGrams - summary.FatConsumedGrams);
        summary.WaterRemainingMl = Math.Max(0, summary.WaterTargetMl - summary.WaterConsumedMl);

        var latestWeight = await _weightLogRepository.GetLatestUserWeightLogAsync(userId, cancellationToken);
        if (latestWeight != null)
        {
            summary.CurrentWeightKg = latestWeight.Weight;
        }

        return summary;
    }

    public async Task<IEnumerable<DailySummaryDto>> GetDailyHistoryAsync(string userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        var summaries = new List<DailySummaryDto>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var summary = await GetDailySummaryAsync(userId, date, cancellationToken);
            summaries.Add(summary);
        }
        return summaries;
    }

    private MealLogDto MapToMealLogDto(MealLog log)
    {
        var dto = new MealLogDto
        {
            Id = log.Id,
            MealType = log.MealType.ToString(),
            Date = log.Date,
            CreatedAt = log.CreatedAt,
            MealItems = new List<MealItemDto>()
        };

        foreach (var item in log.MealItems)
        {
            // Simple multiplier based on unit (assuming per 100g data in DB). Adjust based on exact business logic if needed.
            double multiplier = (double)item.Quantity / 100.0;
            // Handle different units if necessary (e.g. Piece, Cup). For simplicity assuming grams.

            var itemDto = new MealItemDto
            {
                Id = item.Id,
                FoodId = item.FoodId,
                FoodName = item.Food.Name,
                Quantity = item.Quantity,
                Unit = item.Unit.ToString(),
                Calories = (double)(item.Food.Energy ?? 0) * multiplier,
                Protein = (double)(item.Food.Protein ?? 0) * multiplier,
                Carbs = (double)(item.Food.Carbohydrate ?? 0) * multiplier,
                Fat = (double)(item.Food.Fat ?? 0) * multiplier
            };
            dto.MealItems.Add(itemDto);

            dto.TotalCalories += itemDto.Calories;
            dto.TotalProtein += itemDto.Protein;
            dto.TotalCarbs += itemDto.Carbs;
            dto.TotalFat += itemDto.Fat;
        }

        return dto;
    }
}
