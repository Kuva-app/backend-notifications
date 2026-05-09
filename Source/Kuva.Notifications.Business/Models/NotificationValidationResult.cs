namespace Kuva.Notifications.Business.Models;

public sealed class NotificationValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];

    public static NotificationValidationResult Success() => new();

    public static NotificationValidationResult Failure(params string[] errors)
    {
        var result = new NotificationValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }
}
