using System.Net;
using System.Net.Mail;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Enums;
using Microsoft.Extensions.Configuration;

namespace Kuva.Email.Business.Providers;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public EmailProviderType ProviderType => EmailProviderType.Smtp;

    public async Task<EmailSendResult> SendAsync(RenderedEmail email, CancellationToken cancellationToken)
    {
        var host = configuration["Smtp:Host"];
        var fromEmail = configuration["Smtp:FromEmail"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail))
        {
            return EmailSendResult.Fail("SmtpNotConfigured", "SMTP provider is not configured.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, configuration["Smtp:FromName"] ?? "Kuva"),
            Subject = email.Subject,
            Body = email.HtmlBody,
            IsBodyHtml = true
        };

        foreach (var recipient in email.Recipients)
        {
            var address = new MailAddress(recipient.Email, recipient.Name);
            switch (recipient.Type.ToUpperInvariant())
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
            return EmailSendResult.Ok();
        }
        catch (SmtpException ex)
        {
            return EmailSendResult.Fail(ex.StatusCode.ToString(), "SMTP provider failed to send the email.");
        }
    }
}
