using FluentAssertions;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Entities.Dtos;
using Kuva.Email.Entities.Enums;
using Kuva.Email.Service.Controllers;
using Kuva.Email.Tests.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kuva.Email.Tests.Service;

[TestFixture]
public sealed class EmailControllerTests
{
    [Test]
    public async Task SendAsync_WhenBusinessReturnsSent_ShouldReturnAccepted()
    {
        var business = new Mock<IEmailBusiness>();
        business.Setup(x => x.SendAsync(It.IsAny<SendEmailRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendEmailResponseDto
            {
                Id = Guid.NewGuid(),
                Status = EmailRequestStatus.Sent,
                TemplateCode = "ORDER_RECEIVED",
                Message = "Email sent successfully."
            });
        var sut = new EmailController(business.Object);

        var result = await sut.SendAsync(new SendEmailRequestDtoBuilder().Build(), CancellationToken.None);

        result.Should().BeOfType<AcceptedAtActionResult>();
    }

    [Test]
    public async Task GetStatusAsync_WhenNotFound_ShouldReturnNotFound()
    {
        var business = new Mock<IEmailBusiness>();
        business.Setup(x => x.GetStatusAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmailStatusDto?)null);
        var sut = new EmailController(business.Object);

        var result = await sut.GetStatusAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }
}
