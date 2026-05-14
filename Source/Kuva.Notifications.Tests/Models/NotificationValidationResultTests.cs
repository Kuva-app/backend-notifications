using Kuva.Notifications.Business.Models;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Models;

[TestFixture]
public class NotificationValidationResultTests
{
    [Test]
    public void IsValid_NoErrors_ReturnsTrue()
    {
        var result = NotificationValidationResult.Success();

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void IsValid_WithErrors_ReturnsFalse()
    {
        var result = NotificationValidationResult.Failure("error1");

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Success_ReturnsResultWithNoErrors()
    {
        var result = NotificationValidationResult.Success();

        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public void Success_IsValid_ReturnsTrue()
    {
        var result = NotificationValidationResult.Success();

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Failure_SingleError_AddsError()
    {
        var result = NotificationValidationResult.Failure("error1");

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0], Is.EqualTo("error1"));
    }

    [Test]
    public void Failure_MultipleErrors_AddsAllErrors()
    {
        var result = NotificationValidationResult.Failure("error1", "error2", "error3");

        Assert.That(result.Errors, Is.EquivalentTo(["error1", "error2", "error3"]));
    }

    [Test]
    public void Failure_NoErrors_IsValidReturnsTrue()
    {
        var result = NotificationValidationResult.Failure();

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Failure_WithErrors_IsValidReturnsFalse()
    {
        var result = NotificationValidationResult.Failure("e1", "e2");

        Assert.That(result.IsValid, Is.False);
    }
}
