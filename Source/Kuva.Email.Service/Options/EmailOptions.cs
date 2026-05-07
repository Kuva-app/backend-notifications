namespace Kuva.Email.Service.Options;

public sealed class EmailOptions
{
    public string Provider { get; set; } = "Fake";
    public int RetentionDays { get; set; } = 180;
    public int IdempotencyWindowHours { get; set; } = 24;
}
