using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Enums;
using Microsoft.Extensions.Configuration;

namespace Kuva.Email.Business.Providers;

public sealed class SendGridEmailSender(HttpClient httpClient, IConfiguration configuration) : IEmailSender
{
    public EmailProviderType ProviderType => EmailProviderType.SendGrid;

    public async Task<EmailSendResult> SendAsync(RenderedEmail email, CancellationToken cancellationToken)
    {
        var apiKey = configuration["SendGrid:ApiKey"];
        var fromEmail = configuration["SendGrid:FromEmail"] ?? configuration["Smtp:FromEmail"];
        var fromName = configuration["SendGrid:FromName"] ?? configuration["Smtp:FromName"] ?? "Kuva";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail))
        {
            return EmailSendResult.Fail("SendGridNotConfigured", "SendGrid provider is not configured.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
        request.Headers.Authorization = new("Bearer", apiKey);
        request.Content = JsonContent.Create(new SendGridMessage(
            new SendGridAddress(fromEmail, fromName),
            [new SendGridPersonalization(email.Recipients.Where(x => x.Type.Equals("To", StringComparison.OrdinalIgnoreCase)).Select(x => new SendGridAddress(x.Email, x.Name)).ToArray())],
            email.Subject,
            [new SendGridContent("text/html", email.HtmlBody)]));

        var response = await httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode
            ? EmailSendResult.Ok(response.Headers.TryGetValues("X-Message-Id", out var values) ? values.FirstOrDefault() : null)
            : EmailSendResult.Fail(response.StatusCode.ToString(), "SendGrid provider failed to send the email.");
    }

    private sealed record SendGridMessage(
        [property: JsonPropertyName("from")] SendGridAddress From,
        [property: JsonPropertyName("personalizations")] IReadOnlyCollection<SendGridPersonalization> Personalizations,
        [property: JsonPropertyName("subject")] string Subject,
        [property: JsonPropertyName("content")] IReadOnlyCollection<SendGridContent> Content);

    private sealed record SendGridPersonalization([property: JsonPropertyName("to")] IReadOnlyCollection<SendGridAddress> To);
    private sealed record SendGridAddress([property: JsonPropertyName("email")] string Email, [property: JsonPropertyName("name")] string? Name);
    private sealed record SendGridContent([property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("value")] string Value);
}
