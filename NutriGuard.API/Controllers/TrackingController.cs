using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.Tracking;
using NutriGuard.Application.Interfaces.Services;

namespace NutriGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;

    public TrackingController(ITrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    [HttpPost("meals")]
    public async Task<IActionResult> LogMeal([FromBody] LogMealRequestDto request)
    {
        var userId = GetUserId();
        var result = await _trackingService.LogMealAsync(userId, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("water")]
    public async Task<IActionResult> LogWater([FromBody] LogWaterRequestDto request)
    {
        var userId = GetUserId();
        var result = await _trackingService.LogWaterAsync(userId, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("weight")]
    public async Task<IActionResult> LogWeight([FromBody] LogWeightRequestDto request)
    {
        var userId = GetUserId();
        var result = await _trackingService.LogWeightAsync(userId, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("summary/{date}")]
    public async Task<IActionResult> GetDailySummary(DateOnly date)
    {
        var userId = GetUserId();
        var result = await _trackingService.GetDailySummaryAsync(userId, date);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetDailyHistory([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var userId = GetUserId();
        var result = await _trackingService.GetDailyHistoryAsync(userId, startDate, endDate);
        return Ok(result);
    }

    [HttpDelete("meals/{mealLogId}")]
    public async Task<IActionResult> DeleteMeal(int mealLogId)
    {
        var userId = GetUserId();
        var result = await _trackingService.DeleteMealAsync(userId, mealLogId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("water/{waterLogId}")]
    public async Task<IActionResult> DeleteWater(int waterLogId)
    {
        var userId = GetUserId();
        var result = await _trackingService.DeleteWaterAsync(userId, waterLogId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("weight/{weightLogId}")]
    public async Task<IActionResult> DeleteWeight(int weightLogId)
    {
        var userId = GetUserId();
        var result = await _trackingService.DeleteWeightAsync(userId, weightLogId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
