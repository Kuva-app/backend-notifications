using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kuva.Notifications.Service.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public sealed class NotificationsController(INotificationBusiness notificationBusiness) : ControllerBase
{
    [HttpPost("send")]
    [EnableRateLimiting("notification-send")]
    [ProducesResponseType(typeof(SendNotificationResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendAsync([FromBody] SendNotificationRequestDto request, CancellationToken cancellationToken)
    {
        var response = await notificationBusiness.SendAsync(request, cancellationToken);

        return response.Status switch
        {
            NotificationRequestStatus.InvalidVariables => BadRequest(ToError("InvalidVariables", response.Message)),
            NotificationRequestStatus.TemplateNotFound => NotFound(ToError("TemplateNotFound", response.Message)),
            NotificationRequestStatus.Failed => StatusCode(StatusCodes.Status502BadGateway, ToError("NotificationSendFailed", response.Message)),
            NotificationRequestStatus.Sent => Accepted(response)
        };
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NotificationStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var status = await notificationBusiness.GetStatusAsync(id, cancellationToken);
        return status is null ? NotFound() : Ok(status);
    }

    private ErrorResponseDto ToError(string error, string message)
        => new()
        {
            Error = error,
            Message = message,
            CorrelationId = HttpContext.TraceIdentifier
        };
}
