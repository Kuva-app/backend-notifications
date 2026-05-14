using Kuva.Notifications.Repository.Repositories;

namespace Kuva.Notifications.Tests.Repositories;

[TestFixture]
public sealed class UnitOfWorkTests : TestBase
{
    [Test]
    public async Task SaveChangesAsync_WhenNoChanges_ShouldReturnZero()
    {
        await using var dbContext = CreateDbContext();
        var sut = new UnitOfWork(dbContext);

        var result = await sut.SaveChangesAsync(CancellationToken.None);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task SaveChangesAsync_WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        await using var dbContext = CreateDbContext();
        var sut = new UnitOfWork(dbContext);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // InMemory provider does not honour cancellation; verify the token is already cancelled
        // and that SaveChangesAsync propagates it when the context has pending changes.
        var template = new Kuva.Notifications.Tests.Builders.NotificationTemplateBuilder().Build();
        dbContext.NotificationTemplates.Add(template);

        var act = async () => await sut.SaveChangesAsync(cts.Token);

        Assert.That(async () => await act(), Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task SaveChangesAsync_WhenEntityAdded_ShouldReturnNumberOfAffectedRows()
    {
        await using var dbContext = CreateDbContext();
        var sut = new UnitOfWork(dbContext);

        var template = new Kuva.Notifications.Tests.Builders.NotificationTemplateBuilder().Build();
        dbContext.NotificationTemplates.Add(template);

        var result = await sut.SaveChangesAsync(CancellationToken.None);

        Assert.That(result, Is.EqualTo(1));
    }
}
