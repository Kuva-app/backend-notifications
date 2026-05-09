namespace Kuva.Notifications.Business.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}
