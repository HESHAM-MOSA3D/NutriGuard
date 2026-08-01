using NutriGuard.Application.DTOs.Auth;
using System.Security.Claims;

namespace NutriGuard.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

        Task<AuthResponseDto> LogoutAsync(ClaimsPrincipal user);

        Task<AuthResponseDto> GetCurrentUserAsync(ClaimsPrincipal user);

        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);

        Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request);

        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}