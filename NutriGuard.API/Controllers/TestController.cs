using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.Interfaces;

namespace NutriGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{

    private readonly IEmailService _emailService;
    public TestController(IEmailService emailService)
    {
        _emailService = emailService;
    }


    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Message = "JWT Authentication Works Successfully!!!"
        });
    }

    [HttpPost("email")]
    public async Task<IActionResult> SendTestEmail()
    {
        await _emailService.SendEmailAsync(
            "heshamm7700@gmail.com",
            "NutriGuard Test",
            "<h2>Email Service Works Successfully</h2>");

        return Ok("Email Sent");
    }


}