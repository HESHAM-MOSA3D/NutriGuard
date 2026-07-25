namespace NutriGuard.Application.DTOs.HealthProfile;

public sealed class ProfileCompletionDto
{
    public int CompletionPercentage { get; init; }

    public bool IsCompleted { get; init; }

    public IEnumerable<string> MissingFields { get; init; } = [];
}