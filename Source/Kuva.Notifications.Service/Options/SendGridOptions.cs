namespace Kuva.Notifications.Service.Options;

public sealed class SendGridOptions
{
    public string? ApiKey { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Kuva";
}
