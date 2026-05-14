using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Service.Controllers;
using Kuva.Notifications.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kuva.Notifications.Tests.Service;

[TestFixture]
public sealed class NotificationsControllerTests
{
    [Test]
    public async Task SendAsync_WhenBusinessReturnsSent_ShouldReturnAccepted()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.SendAsync(It.IsAny<SendNotificationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendNotificationResponseDto
            {
                Id = Guid.NewGuid(),
                Type = NotificationType.Email,
                TemplateId = Kuva.Notifications.Repository.Context.NotificationsDbContext.OrderReceivedTemplateId,
                Status = NotificationRequestStatus.Sent,
                TemplateCode = "ORDER_RECEIVED",
                Message = "Notification sent successfully."
            });
        var sut = new NotificationsController(business.Object);

        var result = await sut.SendAsync(new SendNotificationRequestDtoBuilder().Build(), CancellationToken.None);

        Assert.That(result, Is.InstanceOf<AcceptedResult>());
    }

    [Test]
    public async Task SendAsync_WhenBusinessReturnsInvalidVariables_ShouldReturnBadRequest()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.SendAsync(It.IsAny<SendNotificationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendNotificationResponseDto
            {
                Status = NotificationRequestStatus.InvalidVariables,
                Message = "Invalid variables."
            });
        var sut = new NotificationsController(business.Object);
        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await sut.SendAsync(new SendNotificationRequestDtoBuilder().Build(), CancellationToken.None);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task SendAsync_WhenBusinessReturnsTemplateNotFound_ShouldReturnNotFound()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.SendAsync(It.IsAny<SendNotificationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendNotificationResponseDto
            {
                Status = NotificationRequestStatus.TemplateNotFound,
                Message = "Template not found."
            });
        var sut = new NotificationsController(business.Object);
        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await sut.SendAsync(new SendNotificationRequestDtoBuilder().Build(), CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task SendAsync_WhenBusinessReturnsFailed_ShouldReturn502()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.SendAsync(It.IsAny<SendNotificationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendNotificationResponseDto
            {
                Status = NotificationRequestStatus.Failed,
                Message = "Send failed."
            });
        var sut = new NotificationsController(business.Object);
        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await sut.SendAsync(new SendNotificationRequestDtoBuilder().Build(), CancellationToken.None);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(502));
    }

    [Test]
    public async Task GetStatusAsync_WhenNotFound_ShouldReturnNotFound()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.GetStatusAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationStatusDto?)null);
        var sut = new NotificationsController(business.Object);

        var result = await sut.GetStatusAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetStatusAsync_WhenFound_ShouldReturnOkWithStatus()
    {
        var statusDto = new NotificationStatusDto();
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.GetStatusAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);
        var sut = new NotificationsController(business.Object);

        var result = await sut.GetStatusAsync(Guid.NewGuid(), CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(statusDto));
    }
}
