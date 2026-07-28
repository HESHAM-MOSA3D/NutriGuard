namespace NutriGuard.Application.DTOs.Tracking;

public class WeightLogResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public WeightLogDto? Data { get; set; }

    public static WeightLogResponseDto Success(WeightLogDto data)
    {
        return new()
        {
            IsSuccess = true,
            Message = "Success",
            Data = data
        };
    }

    public static WeightLogResponseDto Failure(string message)
    {
        return new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
