namespace Kuva.Email.Business.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}
