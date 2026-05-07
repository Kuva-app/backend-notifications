using Kuva.Email.Business.Interfaces;
using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Business.Services;

public sealed class NoopEmailMetrics : IEmailMetrics
{
    public void RequestReceived()
    {
    }

    public void SendCompleted(EmailRequestStatus status, string providerName, double durationSeconds)
    {
    }

    public void ProviderFailure(string providerName)
    {
    }
}
