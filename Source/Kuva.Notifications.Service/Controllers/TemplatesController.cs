using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Kuva.Notifications.Service.Controllers;

[ApiController]
[Route("api/v1/templates")]
public sealed class TemplatesController(INotificationBusiness notificationBusiness) : ControllerBase
{
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var template = await notificationBusiness.GetTemplateByCodeAsync(code, cancellationToken);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] NotificationTemplateDto template, CancellationToken cancellationToken)
    {
        var created = await notificationBusiness.CreateTemplateAsync(template, cancellationToken);
        return Created(ControllerContext.HttpContext.Request.Path, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] NotificationTemplateDto template, CancellationToken cancellationToken)
    {
        var updated = await notificationBusiness.UpdateTemplateAsync(id, template, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetStatusAsync(Guid id, [FromBody] UpdateTemplateStatusDto status, CancellationToken cancellationToken)
    {
        var updated = await notificationBusiness.SetTemplateStatusAsync(id, status.IsActive, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}
