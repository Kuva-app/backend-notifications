using System.Net.Mail;

namespace Kuva.Notifications.Entities.ValueObjects;

public sealed record EmailAddress(string Value)
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            var address = new MailAddress(value);
            return address.Address.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
