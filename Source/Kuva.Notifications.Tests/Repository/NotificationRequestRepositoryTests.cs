using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Repositories;
using Kuva.Notifications.Tests.Builders;

namespace Kuva.Notifications.Tests.Repository;

[TestFixture]
public sealed class NotificationRequestRepositoryTests : TestBase
{
    [Test]
    public async Task GetByIdempotencyKeyAsync_WhenRequestExists_ShouldReturnExistingRequest()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().WithRecipient("cliente@email.com").Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var result = await sut.GetByIdempotencyKeyAsync(
            request.Type,
            request.TemplateId!.Value,
            request.ExternalReference!,
            "cliente@email.com",
            request.CreatedAtUtc.AddMinutes(-1),
            CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(request.Id));
    }

    [Test]
    public async Task UpdateStatusAsync_WhenRequestExists_ShouldUpdateStatus()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().WithRecipient("cliente@email.com").Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        await sut.UpdateStatusAsync(request.Id, NotificationRequestStatus.Failed, "failed", DateTime.UtcNow, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var updated = await sut.GetByIdAsync(request.Id, CancellationToken.None);
        Assert.That(updated!.Status, Is.EqualTo(NotificationRequestStatus.Failed));
        Assert.That(updated.ErrorMessage, Is.EqualTo("failed"));
    }

    [Test]
    public async Task AddAsync_WhenCalled_ShouldPersistRequest()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().Build();
        var sut = new NotificationRequestRepository(dbContext);

        await sut.AddAsync(request, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        Assert.That(dbContext.NotificationRequests.Count(r => r.Id == request.Id), Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_WhenRequestExists_ShouldReturnRequestWithRelatedEntities()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().WithRecipient("test@example.com").Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var result = await sut.GetByIdAsync(request.Id, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(request.Id));
        Assert.That(result.Recipients, Is.Not.Empty);
    }

    [Test]
    public async Task GetByIdAsync_WhenRequestDoesNotExist_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationRequestRepository(dbContext);

        var result = await sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_ShouldIncludeAttempts()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().Build();
        dbContext.NotificationRequests.Add(request);
        var attempt = new NotificationSendAttempt
        {
            Id = Guid.NewGuid(),
            NotificationRequestId = request.Id,
            AttemptNumber = 1,
            StartedAtUtc = DateTime.UtcNow
        };
        dbContext.NotificationSendAttempts.Add(attempt);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var result = await sut.GetByIdAsync(request.Id, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Attempts.Count(a => a.Id == attempt.Id), Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdempotencyKeyAsync_WhenNoMatchExists_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationRequestRepository(dbContext);

        var result = await sut.GetByIdempotencyKeyAsync(
            NotificationType.Email,
            Guid.NewGuid(),
            "ref",
            "nobody@example.com",
            DateTime.UtcNow.AddDays(-1),
            CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdempotencyKeyAsync_WhenCreatedBeforeNotBefore_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().WithRecipient("x@x.com").Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var result = await sut.GetByIdempotencyKeyAsync(
            request.Type,
            request.TemplateId!.Value,
            request.ExternalReference!,
            "x@x.com",
            request.CreatedAtUtc.AddMinutes(1),
            CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdempotencyKeyAsync_WhenRecipientDoesNotMatch_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().WithRecipient("right@x.com").Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var result = await sut.GetByIdempotencyKeyAsync(
            request.Type,
            request.TemplateId!.Value,
            request.ExternalReference!,
            "wrong@x.com",
            request.CreatedAtUtc.AddMinutes(-1),
            CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAttemptCountAsync_WhenNoAttempts_ShouldReturnZero()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationRequestRepository(dbContext);

        var count = await sut.GetAttemptCountAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetAttemptCountAsync_WhenAttemptsExist_ShouldReturnCorrectCount()
    {
        await using var dbContext = CreateDbContext();
        var requestId = Guid.NewGuid();
        dbContext.NotificationSendAttempts.AddRange(
            new NotificationSendAttempt { Id = Guid.NewGuid(), NotificationRequestId = requestId, AttemptNumber = 1, StartedAtUtc = DateTime.UtcNow },
            new NotificationSendAttempt { Id = Guid.NewGuid(), NotificationRequestId = requestId, AttemptNumber = 2, StartedAtUtc = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var count = await sut.GetAttemptCountAsync(requestId, CancellationToken.None);

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAttemptCountAsync_ShouldOnlyCountAttemptsForGivenRequestId()
    {
        await using var dbContext = CreateDbContext();
        var requestId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        dbContext.NotificationSendAttempts.AddRange(
            new NotificationSendAttempt { Id = Guid.NewGuid(), NotificationRequestId = requestId, AttemptNumber = 1, StartedAtUtc = DateTime.UtcNow },
            new NotificationSendAttempt { Id = Guid.NewGuid(), NotificationRequestId = otherId, AttemptNumber = 1, StartedAtUtc = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        var count = await sut.GetAttemptCountAsync(requestId, CancellationToken.None);

        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task AddAttemptAsync_WhenCalled_ShouldPersistAttempt()
    {
        await using var dbContext = CreateDbContext();
        var requestId = Guid.NewGuid();
        var attempt = new NotificationSendAttempt
        {
            Id = Guid.NewGuid(),
            NotificationRequestId = requestId,
            AttemptNumber = 1,
            StartedAtUtc = DateTime.UtcNow
        };

        var sut = new NotificationRequestRepository(dbContext);
        await sut.AddAttemptAsync(attempt, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        Assert.That(dbContext.NotificationSendAttempts.Count(a => a.Id == attempt.Id), Is.EqualTo(1));
    }

    [Test]
    public async Task AddEventAsync_WhenCalled_ShouldPersistEvent()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var notificationEvent = new NotificationEvent
        {
            Id = Guid.NewGuid(),
            NotificationRequestId = request.Id,
            EventType = NotificationEventType.Created,
            CreatedAtUtc = DateTime.UtcNow
        };

        var sut = new NotificationRequestRepository(dbContext);
        await sut.AddEventAsync(notificationEvent, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        Assert.That(dbContext.NotificationEvents.Count(e => e.Id == notificationEvent.Id), Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateStatusAsync_WhenStatusIsSent_ShouldSetSentAtUtc()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var sut = new NotificationRequestRepository(dbContext);
        await sut.UpdateStatusAsync(request.Id, NotificationRequestStatus.Sent, null, now, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var updated = await sut.GetByIdAsync(request.Id, CancellationToken.None);
        Assert.That(updated!.Status, Is.EqualTo(NotificationRequestStatus.Sent));
        Assert.That(updated.SentAtUtc, Is.EqualTo(now));
        Assert.That(updated.ErrorMessage, Is.Null);
        Assert.That(updated.UpdatedAtUtc, Is.EqualTo(now));
    }

    [Test]
    public async Task UpdateStatusAsync_WhenStatusIsNotSent_ShouldNotOverwriteSentAtUtc()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().Build();
        var originalSentAt = request.SentAtUtc;
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var sut = new NotificationRequestRepository(dbContext);
        await sut.UpdateStatusAsync(request.Id, NotificationRequestStatus.Failed, "error", now, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var updated = await sut.GetByIdAsync(request.Id, CancellationToken.None);
        Assert.That(updated!.SentAtUtc, Is.EqualTo(originalSentAt));
        Assert.That(updated.UpdatedAtUtc, Is.EqualTo(now));
    }

    [Test]
    public async Task UpdateStatusAsync_WhenCalled_ShouldUpdateErrorMessage()
    {
        await using var dbContext = CreateDbContext();
        var request = new NotificationRequestBuilder().Build();
        dbContext.NotificationRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationRequestRepository(dbContext);
        await sut.UpdateStatusAsync(request.Id, NotificationRequestStatus.Failed, "some error", DateTime.UtcNow, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var updated = await sut.GetByIdAsync(request.Id, CancellationToken.None);
        Assert.That(updated!.ErrorMessage, Is.EqualTo("some error"));
    }
}
