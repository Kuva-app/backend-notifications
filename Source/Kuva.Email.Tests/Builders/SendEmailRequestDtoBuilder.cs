using Kuva.Email.Entities.Dtos;

namespace Kuva.Email.Tests.Builders;

public sealed class SendEmailRequestDtoBuilder
{
    private readonly SendEmailRequestDto _request = new()
    {
        TemplateCode = "ORDER_RECEIVED",
        ExternalReference = "order-123",
        Source = "Kuva.Tests",
        Recipients =
        [
            new EmailRecipientDto
            {
                Email = "cliente@email.com",
                Name = "Cliente",
                Type = "To"
            }
        ],
        Variables = new()
        {
            ["orderNumber"] = "123",
            ["customerName"] = "Cliente"
        }
    };

    public SendEmailRequestDtoBuilder WithoutVariable(string name)
    {
        _request.Variables.Remove(name);
        return this;
    }

    public SendEmailRequestDtoBuilder WithEmail(string email)
    {
        _request.Recipients[0].Email = email;
        return this;
    }

    public SendEmailRequestDto Build() => _request;
}
