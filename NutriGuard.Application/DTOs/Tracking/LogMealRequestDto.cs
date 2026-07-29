using System;
using System.Collections.Generic;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.DTOs.Tracking;

public class LogMealRequestDto
{
    public MealType MealType { get; set; }
    public DateOnly Date { get; set; }
    public List<CreateMealItemDto> MealItems { get; set; } = new();
}
