using System;

namespace NutriGuard.Application.DTOs.Tracking;

public class LogWaterRequestDto
{
    public double AmountInMl { get; set; }
    public DateOnly Date { get; set; }
}
