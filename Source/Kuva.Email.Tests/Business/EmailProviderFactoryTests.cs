using FluentAssertions;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Business.Services;
using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;
using Kuva.Email.Repository.Interfaces;
using Moq;

namespace Kuva.Email.Tests.Business;

[TestFixture]
public sealed class EmailProviderFactoryTests
{
    [Test]
    public async Task GetActiveProviderAsync_WhenFakeProviderIsActive_ShouldReturnFakeSender()
    {
        var provider = new EmailProvider { Id = Guid.NewGuid(), Name = "Fake", ProviderType = EmailProviderType.Fake, IsActive = true };
        var repository = new Mock<IEmailProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(It.IsAny<CancellationToken>())).ReturnsAsync(provider);

        var fakeSender = new Mock<IEmailSender>();
        fakeSender.SetupGet(x => x.ProviderType).Returns(EmailProviderType.Fake);
        var sut = new EmailProviderFactory(repository.Object, [fakeSender.Object]);

        var result = await sut.GetActiveProviderAsync(CancellationToken.None);

        result.Provider.Should().Be(provider);
        result.Sender.ProviderType.Should().Be(EmailProviderType.Fake);
    }

    [Test]
    public async Task GetActiveProviderAsync_WhenNoProviderExists_ShouldFail()
    {
        var repository = new Mock<IEmailProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(It.IsAny<CancellationToken>())).ReturnsAsync((EmailProvider?)null);
        var sut = new EmailProviderFactory(repository.Object, []);

        var act = () => sut.GetActiveProviderAsync(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
