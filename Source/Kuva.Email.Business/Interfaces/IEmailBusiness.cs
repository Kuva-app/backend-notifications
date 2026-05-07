using Kuva.Email.Entities.Dtos;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailBusiness
{
    Task<SendEmailResponseDto> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken);
    Task<EmailStatusDto?> GetStatusAsync(Guid id, CancellationToken cancellationToken);
    Task<EmailTemplateDto> CreateTemplateAsync(EmailTemplateDto template, CancellationToken cancellationToken);
    Task<EmailTemplateDto?> UpdateTemplateAsync(Guid id, EmailTemplateDto template, CancellationToken cancellationToken);
    Task<bool> SetTemplateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    Task<EmailTemplateDto?> GetTemplateByCodeAsync(string code, CancellationToken cancellationToken);
}
