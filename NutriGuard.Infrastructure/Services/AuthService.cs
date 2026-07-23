using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.DTOs.Auth;
using NutriGuard.Application.Interfaces;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Email;
using NutriGuard.Infrastructure.Persistence;
using NutriGuard.Infrastructure.Security;

namespace NutriGuard.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public AuthService(
        IEmailService emailService,
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _emailService = emailService;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
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

    private static string GenerateOtp()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = AuthMessages.EmailSent
            };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.PasswordResetOtps
                .Where(x => x.UserId == user.Id && !x.IsUsed)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsUsed, true)
                    .SetProperty(x => x.IsVerified, false));

            var otp = GenerateOtp();

            var passwordOtp = new PasswordResetOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                FailedAttempts = 0,
                IsUsed = false,
                IsVerified = false
            };

            _context.PasswordResetOtps.Add(passwordOtp);

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email!,
                "Reset Password",
                EmailTemplates.PasswordResetOtp(otp));

            await transaction.CommitAsync();

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = AuthMessages.EmailSent
            };
        }
        catch
        {
            await transaction.RollbackAsync();

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.EmailSendingFailed
            };
        }
    }

    public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.InvalidOtp
            };
        }

        var otp = await _context.PasswordResetOtps
            .Where(x => x.UserId == user.Id && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.InvalidOtp
            };
        }

        if (otp.IsVerified)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.OtpVerified
            };
        }

        if (DateTime.UtcNow >= otp.ExpiresAt)
        {
            otp.IsUsed = true;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.OtpExpired
            };
        }

        if (otp.OtpCode != request.Otp)
        {
            otp.FailedAttempts++;

            if (otp.FailedAttempts >= 3)
            {
                otp.IsUsed = true;
            }

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = otp.IsUsed
                    ? AuthMessages.TooManyAttempts
                    : AuthMessages.InvalidOtp
            };
        }

        otp.IsVerified = true;

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = AuthMessages.OtpVerified
        };
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.PasswordMismatch
            };
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.UnableToResetPassword
            };
        }

        var otp = await _context.PasswordResetOtps
            .Where(x =>
                x.UserId == user.Id &&
                x.IsVerified &&
                !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.VerificationRequired
            };
        }

        if (DateTime.UtcNow >= otp.ExpiresAt)
        {
            otp.IsUsed = true;
            otp.IsVerified = false;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.OtpExpired
            };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                request.NewPassword);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();

                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(x => x.Description))
                };
            }

            otp.IsUsed = true;
            otp.IsVerified = false;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = AuthMessages.PasswordReset
            };
        }
        catch
        {
            await transaction.RollbackAsync();

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = AuthMessages.UnableToResetPassword
            };
        }
    }
}