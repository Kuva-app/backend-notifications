using FluentAssertions;
using Kuva.Email.Business.Services;
using Kuva.Email.Tests.Builders;

namespace Kuva.Email.Tests.Business;

[TestFixture]
public sealed class EmailValidationServiceTests
{
    private EmailValidationService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new EmailValidationService();

    [Test]
    public void ValidateSendRequest_WhenRequestIsValid_ShouldPass()
    {
        var result = _sut.ValidateSendRequest(new SendEmailRequestDtoBuilder().Build());

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void ValidateSendRequest_WhenEmailIsInvalid_ShouldFail()
    {
        var result = _sut.ValidateSendRequest(new SendEmailRequestDtoBuilder().WithEmail("invalid").Build());

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Contains("invalid", StringComparison.OrdinalIgnoreCase));
    }
}
