using FluentAssertions;
using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Interfaces;
using Kuva.Notifications.Tests.Builders;
using Kuva.Notifications.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kuva.Notifications.Tests.Business;

[TestFixture]
public sealed class NotificationBusinessTests
{
    private Mock<INotificationTemplateRepository> _templateRepository = null!;
    private Mock<INotificationRequestRepository> _requestRepository = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<INotificationProviderFactory> _providerFactory = null!;
    private Mock<INotificationSender> _sender = null!;
    private Mock<INotificationMetrics> _metrics = null!;
    private NotificationBusiness _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _templateRepository = new Mock<INotificationTemplateRepository>();
        _requestRepository = new Mock<INotificationRequestRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _providerFactory = new Mock<INotificationProviderFactory>();
        _sender = new Mock<INotificationSender>();
        _metrics = new Mock<INotificationMetrics>();

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _sender.Setup(x => x.SendAsync(It.IsAny<RenderedNotification>(), It.IsAny<CancellationToken>())).ReturnsAsync(NotificationSendResult.Ok("message-id"));
        _providerFactory.Setup(x => x.GetActiveProviderAsync(NotificationType.Email, It.IsAny<CancellationToken>())).ReturnsAsync(new SelectedNotificationProvider
        {
            Provider = new NotificationProvider { Id = Guid.NewGuid(), Name = "Fake", Type = NotificationType.Email, ProviderType = NotificationProviderType.Fake },
            Sender = _sender.Object
        });

        _sut = new NotificationBusiness(
            _templateRepository.Object,
            _requestRepository.Object,
            _unitOfWork.Object,
            new TemplateRenderer(),
            new NotificationValidationService(),
            _providerFactory.Object,
            new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            _metrics.Object,
            NullLogger<NotificationBusiness>.Instance);
    }

    [Test]
    public async Task SendAsync_WhenTemplateExistsAndProviderSucceeds_ShouldReturnSent()
    {
        _templateRepository.Setup(x => x.GetActiveByIdAsync(Kuva.Notifications.Repository.Context.NotificationsDbContext.OrderReceivedTemplateId, NotificationType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationTemplateBuilder().Build());

        var response = await _sut.SendAsync(new SendNotificationRequestDtoBuilder().Build(), CancellationToken.None);

        response.Status.Should().Be(NotificationRequestStatus.Sent);
        _sender.Verify(x => x.SendAsync(It.IsAny<RenderedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        _requestRepository.Verify(x => x.AddAsync(It.Is<NotificationRequest>(r => r.Status == NotificationRequestStatus.Sent), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendAsync_WhenTemplateDoesNotExist_ShouldReturnTemplateNotFound()
    {
        _templateRepository.Setup(x => x.GetActiveByIdAsync(Kuva.Notifications.Repository.Context.NotificationsDbContext.OrderReceivedTemplateId, NotificationType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate?)null);

        var response = await _sut.SendAsync(new SendNotificationRequestDtoBuilder().Build(), CancellationToken.None);

        response.Status.Should().Be(NotificationRequestStatus.TemplateNotFound);
        _sender.Verify(x => x.SendAsync(It.IsAny<RenderedNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
