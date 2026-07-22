using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.Auth;
using NutriGuard.Application.Interfaces;

namespace NutriGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }
}