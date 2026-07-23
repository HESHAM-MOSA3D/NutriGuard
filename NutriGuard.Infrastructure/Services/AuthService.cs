using Microsoft.AspNetCore.Identity;
using NutriGuard.Application.DTOs.Auth;
using NutriGuard.Application.Interfaces;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Security;

namespace NutriGuard.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "User with this email already exists."

            };
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        ////
        var token = await _jwtTokenGenerator.GenerateTokenAsync(user);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "User registered successfully.",
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            IsProfileCompleted = false
        };




    }

  
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid email or password."
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid email or password."
            };
        }

        var token = await _jwtTokenGenerator.GenerateTokenAsync(user);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Login successful.",
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            IsProfileCompleted = false 
        };
    }
}
