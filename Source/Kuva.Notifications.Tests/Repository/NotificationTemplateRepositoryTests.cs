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

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(template.Id));
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

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveByIdAsync_WhenTypeDoesNotMatch_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetActiveByIdAsync(template.Id, NotificationType.Sms, CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationTemplateRepository(dbContext);

        var result = await sut.GetActiveByIdAsync(Guid.NewGuid(), NotificationType.Email, CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task AddAsync_ShouldPersistTemplate()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        template.Id = Guid.NewGuid();

        var sut = new NotificationTemplateRepository(dbContext);
        await sut.AddAsync(template, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var saved = await dbContext.NotificationTemplates.FindAsync(template.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Code, Is.EqualTo(template.Code));
    }

    [Test]
    public async Task GetByIdAsync_WhenTemplateExists_ShouldReturnTemplate()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetByIdAsync(template.Id, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(template.Id));
    }

    [Test]
    public async Task GetByIdAsync_WhenTemplateDoesNotExist_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationTemplateRepository(dbContext);

        var result = await sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByCodeAsync_WhenActiveTemplateExists_ShouldReturnTemplate()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().WithCode("WELCOME").Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetByCodeAsync("WELCOME", CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Code, Is.EqualTo("WELCOME"));
    }

    [Test]
    public async Task GetByCodeAsync_WhenTemplateIsInactive_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().WithCode("INACTIVE_CODE").Inactive().Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetByCodeAsync("INACTIVE_CODE", CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByCodeAsync_WhenNoTemplateExists_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationTemplateRepository(dbContext);

        var result = await sut.GetByCodeAsync("NONEXISTENT", CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByCodeAsync_WhenMultipleVersionsExist_ShouldReturnHighestVersion()
    {
        await using var dbContext = CreateDbContext();
        var templateV1 = new NotificationTemplateBuilder().WithCode("VERSIONED").Build();
        templateV1.Id = Guid.NewGuid();
        templateV1.Version = 1;

        var templateV2 = new NotificationTemplateBuilder().WithCode("VERSIONED").Build();
        templateV2.Id = Guid.NewGuid();
        templateV2.Version = 2;

        dbContext.NotificationTemplates.AddRange(templateV1, templateV2);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.GetByCodeAsync("VERSIONED", CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Version, Is.EqualTo(2));
    }

    [Test]
    public async Task CodeVersionExistsAsync_WhenMatchExists_ShouldReturnTrue()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.CodeVersionExistsAsync(NotificationType.Email, template.Code, template.Version, null, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CodeVersionExistsAsync_WhenNoMatchExists_ShouldReturnFalse()
    {
        await using var dbContext = CreateDbContext();
        var sut = new NotificationTemplateRepository(dbContext);

        var result = await sut.CodeVersionExistsAsync(NotificationType.Email, "NOCODE", 1, null, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CodeVersionExistsAsync_WhenExceptIdMatchesOnlyRecord_ShouldReturnFalse()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.CodeVersionExistsAsync(NotificationType.Email, template.Code, template.Version, template.Id, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CodeVersionExistsAsync_WhenExceptIdDoesNotMatchRecord_ShouldReturnTrue()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new NotificationTemplateRepository(dbContext);
        var result = await sut.CodeVersionExistsAsync(NotificationType.Email, template.Code, template.Version, Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Update_WhenTemplateModified_ShouldPersistChanges()
    {
        await using var dbContext = CreateDbContext();
        var template = new NotificationTemplateBuilder().Build();
        template.Id = Guid.NewGuid();
        dbContext.NotificationTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var newCode = "UPDATED_CODE";
        template.Code = newCode;

        var sut = new NotificationTemplateRepository(dbContext);
        sut.Update(template);
        await dbContext.SaveChangesAsync();

        var saved = await dbContext.NotificationTemplates.FindAsync(template.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Code, Is.EqualTo(newCode));
    }
}
