using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Repository.Interfaces;

namespace Kuva.Email.Business.Services;

public sealed class EmailProviderFactory(
    IEmailProviderRepository providerRepository,
    IEnumerable<IEmailSender> senders) : IEmailProviderFactory
{
    public async Task<SelectedEmailProvider> GetActiveProviderAsync(CancellationToken cancellationToken)
    {
        var provider = await providerRepository.GetActiveByPriorityAsync(cancellationToken)
            ?? throw new InvalidOperationException("No active email provider configured.");

        var sender = senders.FirstOrDefault(x => x.ProviderType == provider.ProviderType)
            ?? throw new InvalidOperationException($"Email provider sender is not registered: {provider.ProviderType}.");

        return new SelectedEmailProvider
        {
            Provider = provider,
            Sender = sender
        };
    }
}
