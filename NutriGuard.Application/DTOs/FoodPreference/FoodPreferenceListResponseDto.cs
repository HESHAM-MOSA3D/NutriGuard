namespace NutriGuard.Application.DTOs.FoodPreference;

public sealed class FoodPreferenceListResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public IEnumerable<FoodPreferenceDto> Data { get; init; } = [];

    private FoodPreferenceListResponseDto()
    {
    }

    public static FoodPreferenceListResponseDto Success(
        IEnumerable<FoodPreferenceDto> data,
        string message = "Success")
        => new()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };

    public static FoodPreferenceListResponseDto Failure(string message)
        => new()
        {
            IsSuccess = false,
            Message = message,
            Data = []
        };
}