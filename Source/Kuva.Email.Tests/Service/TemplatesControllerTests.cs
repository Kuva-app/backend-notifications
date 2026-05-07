using FluentAssertions;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Entities.Dtos;
using Kuva.Email.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kuva.Email.Tests.Service;

[TestFixture]
public sealed class TemplatesControllerTests
{
    [Test]
    public async Task GetByCodeAsync_WhenTemplateExists_ShouldReturnOk()
    {
        var business = new Mock<IEmailBusiness>();
        business.Setup(x => x.GetTemplateByCodeAsync("ORDER_RECEIVED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailTemplateDto { Code = "ORDER_RECEIVED", Name = "Pedido recebido" });
        var sut = new TemplatesController(business.Object);

        var result = await sut.GetByCodeAsync("ORDER_RECEIVED", CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task SetStatusAsync_WhenTemplateDoesNotExist_ShouldReturnNotFound()
    {
        var business = new Mock<IEmailBusiness>();
        business.Setup(x => x.SetTemplateStatusAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var sut = new TemplatesController(business.Object);

        var result = await sut.SetStatusAsync(Guid.NewGuid(), new UpdateTemplateStatusDto { IsActive = false }, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }
}
