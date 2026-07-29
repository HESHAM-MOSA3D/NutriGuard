namespace NutriGuard.Application.DTOs.Tracking;

public class WaterLogResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public WaterLogDto? Data { get; set; }

    public static WaterLogResponseDto Success(WaterLogDto data)
    {
        return new()
        {
            IsSuccess = true,
            Message = "Success",
            Data = data
        };
    }

    public static WaterLogResponseDto Failure(string message)
    {
        return new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
