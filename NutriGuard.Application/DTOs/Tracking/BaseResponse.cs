namespace NutriGuard.Application.DTOs.Tracking;

public class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static BaseResponse Success(string message = "Success")
    {
        return new()
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static BaseResponse Failure(string message)
    {
        return new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
