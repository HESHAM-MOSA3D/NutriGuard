namespace NutriGuard.Application.DTOs.Food;

public sealed class FoodResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public FoodDto? Data { get; init; }

    private FoodResponseDto()
    {
    }

    public static FoodResponseDto Success(
        FoodDto data,
        string message = "Success")
        => new()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };

    public static FoodResponseDto Failure(string message)
        => new()
        {
            IsSuccess = false,
            Message = message
        };
}