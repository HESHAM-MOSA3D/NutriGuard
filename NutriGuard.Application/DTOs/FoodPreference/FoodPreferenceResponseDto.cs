namespace NutriGuard.Application.DTOs.FoodPreference;

public sealed class FoodPreferenceResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public FoodPreferenceDto? Data { get; init; }

    private FoodPreferenceResponseDto()
    {
    }

    public static FoodPreferenceResponseDto Success(
        FoodPreferenceDto data,
        string message = "Success")
        => new()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };

    public static FoodPreferenceResponseDto Failure(string message)
        => new()
        {
            IsSuccess = false,
            Message = message
        };
}