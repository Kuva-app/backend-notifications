using Kuva.Notifications.Business.Interfaces;

namespace Kuva.Notifications.Business.Services;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
