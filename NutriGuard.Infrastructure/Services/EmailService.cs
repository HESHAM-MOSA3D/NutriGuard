using Microsoft.Extensions.Options;
using NutriGuard.Application.Interfaces;
using NutriGuard.Application.Settings;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NutriGuard.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SendGridSettings _settings;

    public EmailService(IOptions<SendGridSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlContent)
    {
        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(
            _settings.FromEmail,
            _settings.FromName);

        var to = new EmailAddress(toEmail);

        var message = MailHelper.CreateSingleEmail(
            from,
            to,
            subject,
            plainTextContent: "",
            htmlContent: htmlContent);

        await client.SendEmailAsync(message);
    }
}