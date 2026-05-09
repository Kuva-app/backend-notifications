using FluentAssertions;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Repositories;
using Kuva.Notifications.Tests.Builders;

namespace Kuva.Notifications.Tests.Repository;

[TestFixture]
public sealed class NotificationTemplateRepositoryTests : TestBase
{
    [Test]
    public async Task GetActiveByIdAsync_WhenTemplateIsActive_ShouldReturnTemplate()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetActiveByIdAsync(template.Id, NotificationType.Email, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(template.Id);
    }

    [Test]
    public async Task GetActiveByIdAsync_WhenTemplateIsInactive_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Inactive().Build();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetActiveByIdAsync(template.Id, NotificationType.Email, CancellationToken.None);

        result.Should().BeNull();
    }
}
