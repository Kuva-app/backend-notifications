using FluentAssertions;
using Kuva.Email.Entities.Enums;
using Kuva.Email.Repository.Repositories;
using Kuva.Email.Tests.Builders;

namespace Kuva.Email.Tests.Repository;

[TestFixture]
public sealed class EmailRequestRepositoryTests : TestBase
{
    [Test]
    public async Task GetByIdempotencyKeyAsync_WhenRequestExists_ShouldReturnExistingRequest()
    {
        await using var dbContext = CreateDbContext();
        var request = new EmailRequestBuilder().WithRecipient("cliente@email.com").Build();
        dbContext.EmailRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new EmailRequestRepository(dbContext);
        var result = await sut.GetByIdempotencyKeyAsync(
            request.TemplateCode,
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
        var request = new EmailRequestBuilder().WithRecipient("cliente@email.com").Build();
        dbContext.EmailRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var sut = new EmailRequestRepository(dbContext);
        await sut.UpdateStatusAsync(request.Id, EmailRequestStatus.Failed, "failed", DateTime.UtcNow, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var updated = await sut.GetByIdAsync(request.Id, CancellationToken.None);
        updated!.Status.Should().Be(EmailRequestStatus.Failed);
        updated.ErrorMessage.Should().Be("failed");
    }
}
