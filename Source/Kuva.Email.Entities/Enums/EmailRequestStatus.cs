namespace Kuva.Email.Entities.Enums;

public enum EmailRequestStatus
{
    Created = 1,
    TemplateNotFound = 2,
    InvalidVariables = 3,
    PendingSend = 4,
    Sent = 5,
    Failed = 6,
    RetryScheduled = 7,
    Cancelled = 8
}
