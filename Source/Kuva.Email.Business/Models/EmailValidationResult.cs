namespace Kuva.Email.Business.Models;

public sealed class EmailValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];

    public static EmailValidationResult Success() => new();

    public static EmailValidationResult Failure(params string[] errors)
    {
        var result = new EmailValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }
}
