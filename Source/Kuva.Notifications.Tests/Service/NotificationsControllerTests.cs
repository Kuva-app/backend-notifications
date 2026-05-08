using FluentAssertions;
using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Service.Controllers;
using Kuva.Notifications.Tests.Builders;
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

        result.Should().BeOfType<AcceptedAtActionResult>();
    }

    [Test]
    public async Task GetStatusAsync_WhenNotFound_ShouldReturnNotFound()
    {
        var business = new Mock<INotificationBusiness>();
        business.Setup(x => x.GetStatusAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationStatusDto?)null);
        var sut = new NotificationsController(business.Object);

        var result = await sut.GetStatusAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }
}
