using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Repository.Context;

namespace Kuva.Notifications.Tests.Builders;

public sealed class SendNotificationRequestDtoBuilder
{
    private readonly SendNotificationRequestDto _request = new()
    {
        TemplateId = NotificationsDbContext.OrderReceivedTemplateId,
        ExternalReference = "order-123",
        Source = "Kuva.Tests",
        Recipients =
        [
            new NotificationRecipientDto
            {
                Address = "cliente@email.com",
                Name = "Cliente",
                Role = "To"
            }
        ],
        Variables = new()
        {
            ["orderNumber"] = "123",
            ["customerName"] = "Cliente"
        }
    };

    public SendNotificationRequestDtoBuilder WithoutVariable(string name)
    {
        _request.Variables.Remove(name);
        return this;
    }

    public SendNotificationRequestDtoBuilder WithEmail(string email)
    {
        _request.Recipients[0].Address = email;
        return this;
    }

    public SendNotificationRequestDto Build() => _request;
}
