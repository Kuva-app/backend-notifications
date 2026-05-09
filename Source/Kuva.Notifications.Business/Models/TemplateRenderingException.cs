namespace Kuva.Notifications.Business.Models;

public sealed class TemplateRenderingException(IReadOnlyCollection<string> missingVariables)
    : InvalidOperationException("Required variables are missing.")
{
    public IReadOnlyCollection<string> MissingVariables { get; } = missingVariables;
}
