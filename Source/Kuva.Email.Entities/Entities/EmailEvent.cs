using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Entities.Entities;

public sealed class EmailEvent
{
    public Guid Id { get; set; }
    public Guid EmailRequestId { get; set; }
    public EmailEventType EventType { get; set; }
    public string? Description { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public EmailRequest? EmailRequest { get; set; }
}
