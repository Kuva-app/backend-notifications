using Kuva.Notifications.Business.Interfaces;

namespace Kuva.Notifications.Tests.Fakes;

public sealed class FakeClock(DateTime utcNow) : IClock
{
    public DateTime UtcNow { get; set; } = utcNow;
}
