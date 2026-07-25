using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.HealthProfile;
using NutriGuard.Application.Interfaces.Services;

namespace NutriGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthProfileController : ControllerBase
{
    private readonly IHealthProfileService _healthProfileService;

    public HealthProfileController(IHealthProfileService healthProfileService)
    {
        _healthProfileService = healthProfileService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateHealthProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _healthProfileService.CreateAsync(
            userId,
            request,
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _healthProfileService.GetAsync(
            userId,
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateHealthProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _healthProfileService.UpdateAsync(
            userId,
            request,
            cancellationToken);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet("completion-status")]
    public async Task<IActionResult> GetCompletionStatus(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _healthProfileService
            .GetCompletionStatusAsync(
                userId,
                cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}