using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailMetrics
{
    void RequestReceived();
    void SendCompleted(EmailRequestStatus status, string providerName, double durationSeconds);
    void ProviderFailure(string providerName);
}
