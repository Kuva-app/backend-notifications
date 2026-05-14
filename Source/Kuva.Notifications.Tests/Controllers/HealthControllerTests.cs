using Kuva.Notifications.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Controllers;

[TestFixture]
public class HealthControllerTests
{
    private HealthController _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new HealthController();
    }

    [Test]
    public void Get_WhenCalled_ReturnsOkResult()
    {
        // Act
        var result = _sut.Get();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public void Get_WhenCalled_ReturnsServiceName()
    {
        // Act
        var result = (OkObjectResult)_sut.Get();

        // Assert
        var value = result.Value!;
        var serviceProperty = value.GetType().GetProperty("service");
        Assert.That(serviceProperty, Is.Not.Null);
        Assert.That(serviceProperty!.GetValue(value), Is.EqualTo("Kuva.Notifications"));
    }

    [Test]
    public void Get_WhenCalled_ReturnsHealthyStatus()
    {
        // Act
        var result = (OkObjectResult)_sut.Get();

        // Assert
        var value = result.Value!;
        var statusProperty = value.GetType().GetProperty("status");
        Assert.That(statusProperty, Is.Not.Null);
        Assert.That(statusProperty!.GetValue(value), Is.EqualTo("Healthy"));
    }

    [Test]
    public void Get_WhenCalled_ReturnsUtcDateTime()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var result = (OkObjectResult)_sut.Get();
        var after = DateTime.UtcNow;

        // Assert
        var value = result.Value!;
        var utcProperty = value.GetType().GetProperty("utc");
        Assert.That(utcProperty, Is.Not.Null);
        var utc = (DateTime)utcProperty!.GetValue(value)!;
        Assert.Multiple(() => { Assert.That(utc, Is.GreaterThanOrEqualTo(before)); Assert.That(utc, Is.LessThanOrEqualTo(after)); });
    }

    [Test]
    public void Get_WhenCalled_Returns200StatusCode()
    {
        // Act
        var result = (OkObjectResult)_sut.Get();

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }
}
