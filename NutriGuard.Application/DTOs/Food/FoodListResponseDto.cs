namespace NutriGuard.Application.DTOs.Food;

public sealed class FoodListResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public IEnumerable<FoodDto> Data { get; init; } = [];

    private FoodListResponseDto()
    {
    }

    public static FoodListResponseDto Success(
        IEnumerable<FoodDto> data,
        string message = "Success")
        => new()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };

    public static FoodListResponseDto Failure(string message)
        => new()
        {
            IsSuccess = false,
            Message = message,
            Data = []
        };
}