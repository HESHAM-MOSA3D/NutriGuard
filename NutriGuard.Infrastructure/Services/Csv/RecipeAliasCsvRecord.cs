namespace NutriGuard.Infrastructure.Csv;

public class RecipeAliasCsvRecord
{
    public string RecipeName { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;

    public int Language { get; set; }
}