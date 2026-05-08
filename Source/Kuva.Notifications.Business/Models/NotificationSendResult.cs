namespace Kuva.Notifications.Business.Models;

public sealed class NotificationSendResult
{
    public bool Success { get; init; }
    public string? ProviderMessageId { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }

    public static NotificationSendResult Ok(string? providerMessageId = null)
        => new() { Success = true, ProviderMessageId = providerMessageId };

    public static NotificationSendResult Fail(string errorCode, string errorMessage)
        => new() { Success = false, ErrorCode = errorCode, ErrorMessage = errorMessage };
}
