using System.Text.Json;
using System.Text.RegularExpressions;
using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Entities;

namespace Kuva.Notifications.Business.Services;

public sealed partial class TemplateRenderer : ITemplateRenderer
{
    public RenderedNotification Render(NotificationTemplate template, SendNotificationRequestDto request)
    {
        var requiredVariables = DeserializeRequiredVariables(template.RequiredVariablesJson);
        var missing = requiredVariables
            .Where(variable => !request.Variables.TryGetValue(variable, out var value) || string.IsNullOrWhiteSpace(value))
            .ToArray();

        if (missing.Length > 0)
        {
            throw new TemplateRenderingException(missing);
        }

        return new RenderedNotification
        {
            Subject = ReplaceVariables(template.SubjectTemplate, request.Variables),
            HtmlBody = ReplaceVariables(template.HtmlBodyTemplate, request.Variables),
            TextBody = template.TextBodyTemplate is null ? null : ReplaceVariables(template.TextBodyTemplate, request.Variables),
            Recipients = request.Recipients
        };
    }

    private static List<string> DeserializeRequiredVariables(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static string ReplaceVariables(string template, IReadOnlyDictionary<string, string> variables)
        => TemplateVariableRegex().Replace(template, match =>
        {
            var key = match.Groups["name"].Value;
            return variables.TryGetValue(key, out var value) ? value : match.Value;
        });

    [GeneratedRegex(@"\{\{\s*(?<name>[A-Za-z0-9_\-.]+)\s*\}\}", RegexOptions.Compiled)]
    private static partial Regex TemplateVariableRegex();
}
