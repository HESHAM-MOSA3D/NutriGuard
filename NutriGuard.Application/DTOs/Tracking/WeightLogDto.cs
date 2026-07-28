using System;

namespace NutriGuard.Application.DTOs.Tracking;

public class WeightLogDto
{
    public int Id { get; set; }
    public double Weight { get; set; }
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }
}
