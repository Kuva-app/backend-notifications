using System.Text.Json;
using System.Text.RegularExpressions;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Dtos;
using Kuva.Email.Entities.Entities;

namespace Kuva.Email.Business.Services;

public sealed partial class TemplateRenderer : ITemplateRenderer
{
    public RenderedEmail Render(EmailTemplate template, SendEmailRequestDto request)
    {
        var requiredVariables = DeserializeRequiredVariables(template.RequiredVariablesJson);
        var missing = requiredVariables
            .Where(variable => !request.Variables.TryGetValue(variable, out var value) || string.IsNullOrWhiteSpace(value))
            .ToArray();

        if (missing.Length > 0)
        {
            throw new TemplateRenderingException(missing);
        }

        return new RenderedEmail
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
