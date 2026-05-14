using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Enums;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Kuva.Notifications.Business.Providers;

internal class MailKitSender(IConfiguration configuration) : INotificationSender
{
    public NotificationType Type => NotificationType.Email;
    public NotificationProviderType ProviderType => NotificationProviderType.MailKitSmtp;

    private readonly string _defaultFromName = "Kuva";
    private readonly int _defaultPort = 587;

    public async Task<NotificationSendResult> SendAsync(RenderedNotification notification, CancellationToken cancellationToken)
    {
        var host = configuration["Smtp:Host"];
        var fromEmail = configuration["Smtp:FromEmail"];
        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var fromName = configuration["Smtp:FromName"] ?? _defaultFromName;
        var port = int.TryParse(configuration["Smtp:Port"], out var p) ? p : _defaultPort;

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail) || 
            string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return NotificationSendResult.Fail("SmtpNotConfigured", "SMTP provider is not configured.");
        }

        var message = new MimeMessage();

        foreach (var recipient in notification.Recipients)
        {
            switch (recipient.Role.ToUpperInvariant())
            {
                case "CC":
                    message.Cc.Add(new MailboxAddress(recipient.Name, recipient.Address)); break;
                case "BCC":
                    message.Bcc.Add(new MailboxAddress(recipient.Name, recipient.Address)); break;
                default:
                    message.To.Add(new MailboxAddress(recipient.Name, recipient.Address)); break;
            }
        }
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.Subject = notification.Subject;
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = notification.HtmlBody,
            TextBody = notification.TextBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(username, password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            return NotificationSendResult.Ok();
        }
        catch (Exception)
        {
            return NotificationSendResult.Fail("SMTP_ERROR", "SMTP provider failed to send the email.");
        }
    }
}