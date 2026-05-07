namespace Kuva.Email.Entities.Dtos;

public sealed class EmailRecipientDto
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Type { get; set; } = "To";
}
