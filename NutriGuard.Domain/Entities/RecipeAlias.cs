using NutriGuard.Domain.Enums;

namespace NutriGuard.Domain.Entities;

public class RecipeAlias
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public string Alias { get; set; } = string.Empty;

    public AliasLanguage Language { get; set; }
}