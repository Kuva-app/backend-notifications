using Kuva.Email.Business.Interfaces;
using Kuva.Email.Entities.Dtos;
using Kuva.Email.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kuva.Email.Service.Controllers;

[ApiController]
[Route("api/v1/emails")]
public sealed class EmailController(IEmailBusiness emailBusiness) : ControllerBase
{
    [HttpPost("send")]
    [EnableRateLimiting("email-send")]
    [ProducesResponseType(typeof(SendEmailResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendAsync([FromBody] SendEmailRequestDto request, CancellationToken cancellationToken)
    {
        var response = await emailBusiness.SendAsync(request, cancellationToken);

        return response.Status switch
        {
            EmailRequestStatus.InvalidVariables => BadRequest(ToError("InvalidVariables", response.Message)),
            EmailRequestStatus.TemplateNotFound => NotFound(ToError("TemplateNotFound", response.Message)),
            EmailRequestStatus.Failed => StatusCode(StatusCodes.Status502BadGateway, ToError("EmailSendFailed", response.Message)),
            _ => AcceptedAtAction(nameof(GetStatusAsync), new { id = response.Id }, response)
        };
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmailStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var status = await emailBusiness.GetStatusAsync(id, cancellationToken);
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
