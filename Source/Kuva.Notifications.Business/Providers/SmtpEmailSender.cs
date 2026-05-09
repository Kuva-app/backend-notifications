using System.Net;
using System.Net.Mail;
using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Enums;
using Microsoft.Extensions.Configuration;

namespace Kuva.Notifications.Business.Providers;

public sealed class SmtpEmailSender(IConfiguration configuration) : INotificationSender
{
    public NotificationType Type => NotificationType.Email;
    public NotificationProviderType ProviderType => NotificationProviderType.Smtp;

    public async Task<NotificationSendResult> SendAsync(RenderedNotification notification, CancellationToken cancellationToken)
    {
        var host = configuration["Smtp:Host"];
        var fromEmail = configuration["Smtp:FromEmail"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail))
        {
            return NotificationSendResult.Fail("SmtpNotConfigured", "SMTP provider is not configured.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, configuration["Smtp:FromName"] ?? "Kuva"),
            Subject = notification.Subject,
            Body = notification.HtmlBody,
            IsBodyHtml = true
        };

        foreach (var recipient in notification.Recipients)
        {
            var address = new MailAddress(recipient.Address, recipient.Name);
            switch (recipient.Role.ToUpperInvariant())
            {
                case "CC":
                    message.CC.Add(address);
                    break;
                case "BCC":
                    message.Bcc.Add(address);
                    break;
                default:
                    message.To.Add(address);
                    break;
            }
        }

        using var client = new SmtpClient(host, int.TryParse(configuration["Smtp:Port"], out var port) ? port : 587)
        {
            EnableSsl = bool.TryParse(configuration["Smtp:EnableSsl"], out var ssl) ? ssl : true
        };

        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            client.Credentials = new NetworkCredential(username, password);
        }

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            return NotificationSendResult.Ok();
        }
        catch (SmtpException ex)
        {
            return NotificationSendResult.Fail(ex.StatusCode.ToString(), "SMTP provider failed to send the email.");
        }
    }
}
