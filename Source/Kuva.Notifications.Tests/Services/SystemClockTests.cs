using Kuva.Notifications.Business.Services;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Services;

[TestFixture]
public class SystemClockTests
{
    [Test]
    public void UtcNow_WhenCalled_ReturnsCurrentUtcTime()
    {
        var clock = new SystemClock();
        var before = DateTime.UtcNow;

        var result = clock.UtcNow;

        var after = DateTime.UtcNow;
        Assert.Multiple(() => { Assert.That(result, Is.GreaterThanOrEqualTo(before)); Assert.That(result, Is.LessThanOrEqualTo(after)); });
    }

    [Test]
    public void UtcNow_WhenCalled_ReturnsDateTimeWithUtcKind()
    {
        var clock = new SystemClock();

        var result = clock.UtcNow;

        Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
    }
}
