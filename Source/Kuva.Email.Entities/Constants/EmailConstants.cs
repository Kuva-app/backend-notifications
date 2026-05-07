namespace Kuva.Email.Entities.Constants;

public static class EmailConstants
{
    public const int MaxRecipients = 100;
    public const int MaxTemplateCodeLength = 100;
    public const int MaxEmailLength = 320;
    public const int MaxVariableValueLength = 4000;
    public const int MaxMetadataValueLength = 4000;
    public const string DefaultRecipientType = "To";

    public static readonly string[] AllowedRecipientTypes = ["To", "Cc", "Bcc"];
}
