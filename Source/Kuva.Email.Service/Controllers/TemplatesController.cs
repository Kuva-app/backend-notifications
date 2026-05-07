using Kuva.Email.Business.Interfaces;
using Kuva.Email.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kuva.Email.Service.Controllers;

[ApiController]
[Route("api/v1/templates")]
public sealed class TemplatesController(IEmailBusiness emailBusiness) : ControllerBase
{
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(EmailTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var template = await emailBusiness.GetTemplateByCodeAsync(code, cancellationToken);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EmailTemplateDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] EmailTemplateDto template, CancellationToken cancellationToken)
    {
        var created = await emailBusiness.CreateTemplateAsync(template, cancellationToken);
        return CreatedAtAction(nameof(GetByCodeAsync), new { code = created.Code }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(EmailTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] EmailTemplateDto template, CancellationToken cancellationToken)
    {
        var updated = await emailBusiness.UpdateTemplateAsync(id, template, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetStatusAsync(Guid id, [FromBody] UpdateTemplateStatusDto status, CancellationToken cancellationToken)
    {
        var updated = await emailBusiness.SetTemplateStatusAsync(id, status.IsActive, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}
