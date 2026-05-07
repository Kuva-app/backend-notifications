namespace Kuva.Email.Business.Models;

public sealed class EmailSendResult
{
    public bool Success { get; init; }
    public string? ProviderMessageId { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }

    public static EmailSendResult Ok(string? providerMessageId = null)
        => new() { Success = true, ProviderMessageId = providerMessageId };

    public static EmailSendResult Fail(string errorCode, string errorMessage)
        => new() { Success = false, ErrorCode = errorCode, ErrorMessage = errorMessage };
}
