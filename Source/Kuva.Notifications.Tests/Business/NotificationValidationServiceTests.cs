using FluentAssertions;
using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Tests.Builders;

namespace Kuva.Notifications.Tests.Business;

[TestFixture]
public sealed class NotificationValidationServiceTests
{
    private NotificationValidationService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new NotificationValidationService();

    [Test]
    public void ValidateSendRequest_WhenRequestIsValid_ShouldPass()
    {
        var result = _sut.ValidateSendRequest(new SendNotificationRequestDtoBuilder().Build());

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void ValidateSendRequest_WhenEmailIsInvalid_ShouldFail()
    {
        var result = _sut.ValidateSendRequest(new SendNotificationRequestDtoBuilder().WithEmail("invalid").Build());

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Contains("invalid", StringComparison.OrdinalIgnoreCase));
    }
}
