using Kuva.Email.Business.Models;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailProviderFactory
{
    Task<SelectedEmailProvider> GetActiveProviderAsync(CancellationToken cancellationToken);
}
