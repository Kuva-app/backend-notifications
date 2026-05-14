using Kuva.Notifications.Business.Models;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Models;

[TestFixture]
public class NotificationSendResultTests
{
    [Test]
    public void Ok_WithNoArguments_ReturnSuccessTrue()
    {
        var result = NotificationSendResult.Ok();

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void Ok_WithNoArguments_ReturnsNullProviderMessageId()
    {
        var result = NotificationSendResult.Ok();

        Assert.That(result.ProviderMessageId, Is.Null);
    }

    [Test]
    public void Ok_WithProviderMessageId_ReturnsThatId()
    {
        var result = NotificationSendResult.Ok("msg-123");

        Assert.That(result.ProviderMessageId, Is.EqualTo("msg-123"));
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void Ok_WithNullProviderMessageId_ReturnsNullId()
    {
        var result = NotificationSendResult.Ok(null);

        Assert.That(result.ProviderMessageId, Is.Null);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void Fail_WithErrorCodeAndMessage_ReturnsSuccessFalse()
    {
        var result = NotificationSendResult.Fail("ERR_001", "Something went wrong");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void Fail_WithErrorCodeAndMessage_ReturnsCorrectErrorCode()
    {
        var result = NotificationSendResult.Fail("ERR_001", "Something went wrong");

        Assert.That(result.ErrorCode, Is.EqualTo("ERR_001"));
    }

    [Test]
    public void Fail_WithErrorCodeAndMessage_ReturnsCorrectErrorMessage()
    {
        var result = NotificationSendResult.Fail("ERR_001", "Something went wrong");

        Assert.That(result.ErrorMessage, Is.EqualTo("Something went wrong"));
    }
}
