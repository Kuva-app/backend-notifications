namespace Kuva.Email.Entities.Dtos;

public sealed class ErrorResponseDto
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Details { get; set; } = [];
    public string? CorrelationId { get; set; }
}
