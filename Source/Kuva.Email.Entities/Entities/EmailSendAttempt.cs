namespace Kuva.Email.Entities.Entities;

public sealed class EmailSendAttempt
{
    public Guid Id { get; set; }
    public Guid EmailRequestId { get; set; }
    public Guid? EmailProviderId { get; set; }
    public int AttemptNumber { get; set; }
    public bool Success { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }

    public EmailRequest? EmailRequest { get; set; }
    public EmailProvider? EmailProvider { get; set; }
}
