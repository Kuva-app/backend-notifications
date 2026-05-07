using FluentAssertions;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Business.Services;
using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;
using Kuva.Email.Repository.Interfaces;
using Kuva.Email.Tests.Builders;
using Kuva.Email.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kuva.Email.Tests.Business;

[TestFixture]
public sealed class EmailBusinessTests
{
    private Mock<IEmailTemplateRepository> _templateRepository = null!;
    private Mock<IEmailRequestRepository> _requestRepository = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IEmailProviderFactory> _providerFactory = null!;
    private Mock<IEmailSender> _sender = null!;
    private Mock<IEmailMetrics> _metrics = null!;
    private EmailBusiness _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _templateRepository = new Mock<IEmailTemplateRepository>();
        _requestRepository = new Mock<IEmailRequestRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _providerFactory = new Mock<IEmailProviderFactory>();
        _sender = new Mock<IEmailSender>();
        _metrics = new Mock<IEmailMetrics>();

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _sender.Setup(x => x.SendAsync(It.IsAny<RenderedEmail>(), It.IsAny<CancellationToken>())).ReturnsAsync(EmailSendResult.Ok("message-id"));
        _providerFactory.Setup(x => x.GetActiveProviderAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new SelectedEmailProvider
        {
            Provider = new EmailProvider { Id = Guid.NewGuid(), Name = "Fake", ProviderType = EmailProviderType.Fake },
            Sender = _sender.Object
        });

        _sut = new EmailBusiness(
            _templateRepository.Object,
            _requestRepository.Object,
            _unitOfWork.Object,
            new TemplateRenderer(),
            new EmailValidationService(),
            _providerFactory.Object,
            new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            _metrics.Object,
            NullLogger<EmailBusiness>.Instance);
    }

    [Test]
    public async Task SendAsync_WhenTemplateExistsAndProviderSucceeds_ShouldReturnSent()
    {
        _templateRepository.Setup(x => x.GetActiveByCodeAsync("ORDER_RECEIVED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailTemplateBuilder().Build());

        var response = await _sut.SendAsync(new SendEmailRequestDtoBuilder().Build(), CancellationToken.None);

        response.Status.Should().Be(EmailRequestStatus.Sent);
        _sender.Verify(x => x.SendAsync(It.IsAny<RenderedEmail>(), It.IsAny<CancellationToken>()), Times.Once);
        _requestRepository.Verify(x => x.AddAsync(It.Is<EmailRequest>(r => r.Status == EmailRequestStatus.PendingSend), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendAsync_WhenTemplateDoesNotExist_ShouldReturnTemplateNotFound()
    {
        _templateRepository.Setup(x => x.GetActiveByCodeAsync("ORDER_RECEIVED", It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmailTemplate?)null);

        var response = await _sut.SendAsync(new SendEmailRequestDtoBuilder().Build(), CancellationToken.None);

        response.Status.Should().Be(EmailRequestStatus.TemplateNotFound);
        _sender.Verify(x => x.SendAsync(It.IsAny<RenderedEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
