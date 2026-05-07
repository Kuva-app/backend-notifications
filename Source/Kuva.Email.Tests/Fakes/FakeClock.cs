using Kuva.Email.Business.Interfaces;

namespace Kuva.Email.Tests.Fakes;

public sealed class FakeClock(DateTime utcNow) : IClock
{
    public DateTime UtcNow { get; set; } = utcNow;
}
