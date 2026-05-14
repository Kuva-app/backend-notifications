using Kuva.Notifications.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Notifications.Tests.Context;

[TestFixture]
public sealed class NotificationsDbContextTests
{
    private NotificationsDbContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new NotificationsDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void NotificationTemplates_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationTemplates;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationTemplates_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationTemplates;
        var second = _context.NotificationTemplates;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }

    [Test]
    public void NotificationRequests_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationRequests;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationRequests_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationRequests;
        var second = _context.NotificationRequests;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }

    [Test]
    public void NotificationRecipients_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationRecipients;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationRecipients_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationRecipients;
        var second = _context.NotificationRecipients;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }

    [Test]
    public void NotificationAttachments_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationAttachments;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationAttachments_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationAttachments;
        var second = _context.NotificationAttachments;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }

    [Test]
    public void NotificationProviders_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationProviders;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationProviders_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationProviders;
        var second = _context.NotificationProviders;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }

    [Test]
    public void NotificationSendAttempts_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationSendAttempts;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationSendAttempts_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationSendAttempts;
        var second = _context.NotificationSendAttempts;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }

    [Test]
    public void NotificationEvents_WhenAccessed_ReturnsDbSet()
    {
        // Act
        var result = _context.NotificationEvents;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void NotificationEvents_WhenAccessedMultipleTimes_ReturnsSameType()
    {
        // Act
        var first = _context.NotificationEvents;
        var second = _context.NotificationEvents;

        // Assert
        Assert.That(first.GetType(), Is.EqualTo(second.GetType()));
    }
}
