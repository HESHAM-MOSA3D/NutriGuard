using NutriGuard.Domain.Enums;

namespace NutriGuard.Domain.Entities;

public class MealLog
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public MealType MealType { get; set; }

    public DateOnly Date { get; set; }

    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MealLogItem> MealItems { get; set; }
        = new List<MealLogItem>();
}