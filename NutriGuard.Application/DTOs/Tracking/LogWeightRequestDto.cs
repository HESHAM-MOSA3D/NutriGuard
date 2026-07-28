using System;

namespace NutriGuard.Application.DTOs.Tracking;

public class LogWeightRequestDto
{
    public double Weight { get; set; }
    public DateOnly Date { get; set; }
}
