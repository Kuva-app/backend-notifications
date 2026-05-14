using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kuva.Notifications.Tests.Service;

[TestFixture]
public sealed class TemplatesControllerTests
{
    [Test]
    public async Task GetByCodeAsync_WhenTemplateExists_ShouldReturnOk()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.GetTemplateByCodeAsync("ORDER_RECEIVED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationTemplateDto { Code = "ORDER_RECEIVED", Name = "Pedido recebido" });
        var sut = new TemplatesController(business.Object);

        var result = await sut.GetByCodeAsync("ORDER_RECEIVED", CancellationToken.None);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task SetStatusAsync_WhenTemplateDoesNotExist_ShouldReturnNotFound()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.SetTemplateStatusAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var sut = new TemplatesController(business.Object);

        var result = await sut.SetStatusAsync(Guid.NewGuid(), new UpdateTemplateStatusDto { IsActive = false }, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetByCodeAsync_WhenTemplateDoesNotExist_ShouldReturnNotFound()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.GetTemplateByCodeAsync("UNKNOWN", It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplateDto?)null);
        var sut = new TemplatesController(business.Object);

        var result = await sut.GetByCodeAsync("UNKNOWN", CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateAsync_WhenCalled_ShouldReturnCreated()
    {
        var business = new Mock<INotificationBusiness>();
        var dto = new NotificationTemplateDto { Code = "NEW_CODE", Name = "New Template" };
        business.Setup(x => x.CreateTemplateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);
        var sut = new TemplatesController(business.Object);
        sut.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
        };

        var result = await sut.CreateAsync(dto, CancellationToken.None);

        var createdResult = result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.Value, Is.EqualTo(dto));
    }

    [Test]
    public async Task UpdateAsync_WhenTemplateExists_ShouldReturnOk()
    {
        var business = new Mock<INotificationBusiness>();
        var id = Guid.NewGuid();
        var dto = new NotificationTemplateDto { Code = "CODE", Name = "Name" };
        business.Setup(x => x.UpdateTemplateAsync(id, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);
        var sut = new TemplatesController(business.Object);

        var result = await sut.UpdateAsync(id, dto, CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(dto));
    }

    [Test]
    public async Task UpdateAsync_WhenTemplateDoesNotExist_ShouldReturnNotFound()
    {
        var business = new Mock<INotificationBusiness>();
        var id = Guid.NewGuid();
        var dto = new NotificationTemplateDto();
        business.Setup(x => x.UpdateTemplateAsync(id, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplateDto?)null);
        var sut = new TemplatesController(business.Object);

        var result = await sut.UpdateAsync(id, dto, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task SetStatusAsync_WhenTemplateExists_ShouldReturnNoContent()
    {
        var business = new Mock<INotificationBusiness>();
        var id = Guid.NewGuid();
        business.Setup(x => x.SetTemplateStatusAsync(id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var sut = new TemplatesController(business.Object);

        var result = await sut.SetStatusAsync(id, new UpdateTemplateStatusDto { IsActive = true }, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}
