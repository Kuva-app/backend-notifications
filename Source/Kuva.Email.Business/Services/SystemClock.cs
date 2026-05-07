using Kuva.Email.Business.Interfaces;

namespace Kuva.Email.Business.Services;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
