using System;
using System.Collections.Generic;

namespace NutriGuard.Application.DTOs.Tracking;

public class DailySummaryDto
{
    public DateOnly Date { get; set; }

    // Calories Tracking
    public double CaloriesTarget { get; set; }
    public double CaloriesConsumed { get; set; }
    public double CaloriesRemaining { get; set; }

    // Macronutrients Tracking
    public double ProteinTargetGrams { get; set; }
    public double ProteinConsumedGrams { get; set; }
    public double ProteinRemainingGrams { get; set; }

    public double CarbsTargetGrams { get; set; }
    public double CarbsConsumedGrams { get; set; }
    public double CarbsRemainingGrams { get; set; }

    public double FatTargetGrams { get; set; }
    public double FatConsumedGrams { get; set; }
    public double FatRemainingGrams { get; set; }

    // Water Tracking
    public double WaterTargetMl { get; set; }
    public double WaterConsumedMl { get; set; }
    public double WaterRemainingMl { get; set; }

    // Weight Tracking
    public double? CurrentWeightKg { get; set; }

    // Detailed Meals logged today
    public List<MealLogDto> Meals { get; set; } = new();
}
