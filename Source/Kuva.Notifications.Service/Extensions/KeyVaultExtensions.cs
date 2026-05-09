using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace Kuva.Notifications.Service.Extensions;

public static class KeyVaultExtensions
{
    public static IConfigurationBuilder AddKeyVaultIfConfigured(this IConfigurationBuilder configurationBuilder)
    {
        var configuration = configurationBuilder.Build();
        var keyVaultUri = configuration["KeyVault:Uri"];

        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            configurationBuilder.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential(), new AzureKeyVaultConfigurationOptions());
        }

        return configurationBuilder;
    }
}
