using System;

namespace NutriGuard.Application.DTOs.Tracking;

public class WaterLogDto
{
    public int Id { get; set; }
    public double AmountInMl { get; set; }
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }
}
