using FluentAssertions;
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

        result.Should().NotBeNull();
        result!.Id.Should().Be(request.Id);
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
        updated!.Status.Should().Be(NotificationRequestStatus.Failed);
        updated.ErrorMessage.Should().Be("failed");
    }
}
