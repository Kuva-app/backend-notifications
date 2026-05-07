using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Dtos;

namespace Kuva.Email.Business.Interfaces;

public interface ITemplateRenderer
{
    RenderedEmail Render(EmailTemplate template, SendEmailRequestDto request);
}
