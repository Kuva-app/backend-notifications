using FluentAssertions;
using Kuva.Email.Repository.Repositories;
using Kuva.Email.Tests.Builders;

namespace Kuva.Email.Tests.Repository;

[TestFixture]
public sealed class EmailTemplateRepositoryTests : TestBase
{
    [Test]
    public async Task GetActiveByCodeAsync_WhenTemplateIsActive_ShouldReturnTemplate()
    {
        await using var dbContext = CreateDbContext();
        var template = new EmailTemplateBuilder().Build();
        dbContext.EmailTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new EmailTemplateRepository(dbContext);
        var result = await sut.GetActiveByCodeAsync(template.Code, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(template.Id);
    }

    [Test]
    public async Task GetActiveByCodeAsync_WhenTemplateIsInactive_ShouldReturnNull()
    {
        await using var dbContext = CreateDbContext();
        var template = new EmailTemplateBuilder().Inactive().Build();
        dbContext.EmailTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        var sut = new EmailTemplateRepository(dbContext);
        var result = await sut.GetActiveByCodeAsync(template.Code, CancellationToken.None);

        result.Should().BeNull();
    }
}
