using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Repositories;

[TestFixture]
public class NotificationProviderRepositoryTests
{
    private NotificationsDbContext _dbContext = null!;
    private NotificationProviderRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new NotificationsDbContext(options);
        _sut = new NotificationProviderRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    // GetActiveByPriorityAsync tests

    [Test]
    public async Task GetActiveByPriorityAsync_NoProviders_ReturnsNull()
    {
        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveByPriorityAsync_ActiveProviderMatchingType_ReturnsProvider()
    {
        var provider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Email Provider",
            Type = NotificationType.Email,
            IsActive = true,
            Priority = 1
        };
        _dbContext.NotificationProviders.Add(provider);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(provider.Id));
    }

    [Test]
    public async Task GetActiveByPriorityAsync_InactiveProvider_ReturnsNull()
    {
        var provider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Email Provider",
            Type = NotificationType.Email,
            IsActive = false,
            Priority = 1
        };
        _dbContext.NotificationProviders.Add(provider);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveByPriorityAsync_WrongType_ReturnsNull()
    {
        var provider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "SMS Provider",
            Type = NotificationType.Sms,
            IsActive = true,
            Priority = 1
        };
        _dbContext.NotificationProviders.Add(provider);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveByPriorityAsync_MultipleActiveProviders_ReturnsLowestPriority()
    {
        var lowPriority = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Low Priority",
            Type = NotificationType.Email,
            IsActive = true,
            Priority = 1
        };
        var highPriority = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "High Priority",
            Type = NotificationType.Email,
            IsActive = true,
            Priority = 10
        };
        _dbContext.NotificationProviders.AddRange(lowPriority, highPriority);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(lowPriority.Id));
    }

    [Test]
    public async Task GetActiveByPriorityAsync_MixedActiveAndInactiveProviders_ReturnsOnlyActiveLowestPriority()
    {
        var inactiveProvider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Inactive",
            Type = NotificationType.Email,
            IsActive = false,
            Priority = 1
        };
        var activeProvider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Active",
            Type = NotificationType.Email,
            IsActive = true,
            Priority = 5
        };
        _dbContext.NotificationProviders.AddRange(inactiveProvider, activeProvider);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(activeProvider.Id));
    }

    [Test]
    public async Task GetActiveByPriorityAsync_ProvidersOfDifferentTypes_ReturnsOnlyMatchingType()
    {
        var emailProvider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Email",
            Type = NotificationType.Email,
            IsActive = true,
            Priority = 1
        };
        var smsProvider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "SMS",
            Type = NotificationType.Sms,
            IsActive = true,
            Priority = 1
        };
        _dbContext.NotificationProviders.AddRange(emailProvider, smsProvider);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetActiveByPriorityAsync(NotificationType.Sms, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(smsProvider.Id));
    }

    // GetByIdAsync tests

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsProvider()
    {
        var provider = new NotificationProvider
        {
            Id = Guid.NewGuid(),
            Name = "Test Provider",
            Type = NotificationType.Email,
            IsActive = true,
            Priority = 1
        };
        _dbContext.NotificationProviders.Add(provider);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(provider.Id, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(provider.Id));
    }

    [Test]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_CorrectIdAmongMultiple_ReturnsMatchingProvider()
    {
        var provider1 = new NotificationProvider { Id = Guid.NewGuid(), Name = "Provider 1", Type = NotificationType.Email };
        var provider2 = new NotificationProvider { Id = Guid.NewGuid(), Name = "Provider 2", Type = NotificationType.Sms };
        _dbContext.NotificationProviders.AddRange(provider1, provider2);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(provider2.Id, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(provider2.Id));
        Assert.That(result.Name, Is.EqualTo("Provider 2"));
    }
}
