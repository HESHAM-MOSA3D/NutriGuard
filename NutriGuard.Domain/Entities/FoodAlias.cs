using NutriGuard.Domain.Enums;

namespace NutriGuard.Domain.Entities;

public class FoodAlias
{
    public int Id { get; set; }

    public int FoodId { get; set; }

    public Food Food { get; set; } = null!;

    public string Alias { get; set; } = string.Empty;

    public AliasLanguage Language { get; set; }
}