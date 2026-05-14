using Kuva.Notifications.Entities.ValueObjects;
using NUnit.Framework;

namespace Kuva.Notifications.EFMigrations.UnitTests.ValueObjects;

[TestFixture]
public class EmailAddressTests
{
    [Test]
    public void IsValid_NullValue_ReturnsFalse()
    {
        var result = EmailAddress.IsValid(null);
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_EmptyString_ReturnsFalse()
    {
        var result = EmailAddress.IsValid(string.Empty);
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_WhitespaceString_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("   ");
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_ValidEmail_ReturnsTrue()
    {
        var result = EmailAddress.IsValid("user@example.com");
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_InvalidEmail_NoAtSign_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("notanemail");
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_InvalidEmail_MissingDomain_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("user@");
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_EmailWithDisplayName_ReturnsFalse()
    {
        // MailAddress parses "Display Name <user@example.com>" but address != value
        var result = EmailAddress.IsValid("Display Name <user@example.com>");
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_ValidEmailUpperCase_ReturnsTrue()
    {
        // OrdinalIgnoreCase: "USER@EXAMPLE.COM" should match MailAddress.Address
        var result = EmailAddress.IsValid("USER@EXAMPLE.COM");
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_EmailWithLeadingSpace_ReturnsFalse()
    {
        var result = EmailAddress.IsValid(" user@example.com");
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_EmailWithTrailingSpace_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("user@example.com ");
        Assert.That(result, Is.False);
    }
}
