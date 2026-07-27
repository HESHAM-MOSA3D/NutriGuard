namespace NutriGuard.Infrastructure.Csv;

public class RecipeCsvRecord
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PreparationTimeMinutes { get; set; }
}