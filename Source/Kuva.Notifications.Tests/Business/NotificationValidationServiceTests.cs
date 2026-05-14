using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Tests.Builders;

namespace Kuva.Notifications.Tests.Business;

[TestFixture]
public sealed class NotificationValidationServiceTests
{
    private NotificationValidationService _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new NotificationValidationService();

    [Test]
    public void ValidateSendRequest_WhenRequestIsValid_ShouldPass()
    {
        var result = _sut.ValidateSendRequest(new SendNotificationRequestDtoBuilder().Build());

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSendRequest_WhenEmailIsInvalid_ShouldFail()
    {
        var result = _sut.ValidateSendRequest(new SendNotificationRequestDtoBuilder().WithEmail("invalid").Build());

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<string>(x => x.Contains("invalid", StringComparison.OrdinalIgnoreCase)));
    }

    // ValidateSendRequest – notification type
    [Test]
    public void ValidateSendRequest_WhenTypeIsInvalid_ShouldAddTypeError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Type = (NotificationType)999;

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Notification type is invalid."));
    }

    // ValidateSendRequest – template id
    [Test]
    public void ValidateSendRequest_WhenTemplateIdIsEmpty_ShouldAddTemplateIdError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.TemplateId = Guid.Empty;

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("TemplateId is required."));
    }

    // ValidateSendRequest – priority
    [Test]
    public void ValidateSendRequest_WhenPriorityIsInvalid_ShouldAddPriorityError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Priority = (NotificationPriority)999;

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Priority is invalid."));
    }

    // ValidateSendRequest – recipients empty
    [Test]
    public void ValidateSendRequest_WhenRecipientsIsEmpty_ShouldAddRecipientsError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Recipients = [];

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("At least one recipient is required."));
    }

    // ValidateSendRequest – recipients null
    [Test]
    public void ValidateSendRequest_WhenRecipientsIsNull_ShouldAddRecipientsError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Recipients = null!;

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("At least one recipient is required."));
    }

    // ValidateSendRequest – too many recipients
    [Test]
    public void ValidateSendRequest_WhenRecipientsExceedMax_ShouldAddMaxRecipientsError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Recipients = Enumerable.Range(0, 101)
            .Select(_ => new NotificationRecipientDto { Address = "a@b.com", Name = "X", Role = "To" })
            .ToList();

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<string>(e => e.Contains("Recipients cannot exceed")));
    }

    // ValidateSendRequest – email type missing "To" recipient
    [Test]
    public void ValidateSendRequest_WhenEmailTypeHasNoToRecipient_ShouldAddToRecipientError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Type = NotificationType.Email;
        request.Recipients = [new NotificationRecipientDto { Address = "a@b.com", Name = "X", Role = "Cc" }];

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("At least one To recipient is required."));
    }

    // ValidateSendRequest – recipient address empty
    [Test]
    public void ValidateSendRequest_WhenRecipientAddressIsEmpty_ShouldAddAddressError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Recipients = [new NotificationRecipientDto { Address = "", Name = "X", Role = "To" }];

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Recipient address is required."));
    }

    // ValidateSendRequest – recipient address too long
    [Test]
    public void ValidateSendRequest_WhenRecipientAddressIsTooLong_ShouldAddAddressTooLongError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        var longAddress = new string('a', 321) + "@b.com";
        request.Recipients = [new NotificationRecipientDto { Address = longAddress, Name = "X", Role = "To" }];

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<string>(e => e.Contains("Recipient address is too long")));
    }

    // ValidateSendRequest – invalid role for email
    [Test]
    public void ValidateSendRequest_WhenEmailRecipientRoleIsInvalid_ShouldAddRoleError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Type = NotificationType.Email;
        request.Recipients =
        [
            new NotificationRecipientDto { Address = "a@b.com", Name = "X", Role = "To" },
            new NotificationRecipientDto { Address = "b@b.com", Name = "Y", Role = "InvalidRole" }
        ];

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<string>(e => e.Contains("Recipient role is invalid")));
    }

    // ValidateSendRequest – variables null
    [Test]
    public void ValidateSendRequest_WhenVariablesIsNull_ShouldAddVariablesError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Variables = null!;

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Variables cannot be null."));
    }

    // ValidateSendRequest – variable value too long
    [Test]
    public void ValidateSendRequest_WhenVariableValueIsTooLong_ShouldAddVariableValueError()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Variables = new Dictionary<string, string> { ["key"] = new string('x', 4001) };

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<string>(e => e.Contains("Variable value is too long")));
    }

    // ValidateSendRequest – variable value null is allowed
    [Test]
    public void ValidateSendRequest_WhenVariableValueIsNull_ShouldPass()
    {
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Variables = new Dictionary<string, string> { ["key"] = null! };

        var result = _sut.ValidateSendRequest(request);

        Assert.That(result.Errors, Has.None.Matches<string>(e => e.Contains("Variable value is too long")));
    }

    // ValidateTemplate – valid template
    [Test]
    public void ValidateTemplate_WhenTemplateIsValid_ShouldPass()
    {
        var template = new NotificationTemplateDto
        {
            Code = "ORDER_RECEIVED",
            Name = "Order Received",
            Type = NotificationType.Email,
            SubjectTemplate = "Your order",
            HtmlBodyTemplate = "<p>Hello</p>",
            Version = 1
        };

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.True);
    }

    // ValidateTemplate – code empty
    [Test]
    public void ValidateTemplate_WhenCodeIsEmpty_ShouldAddCodeError()
    {
        var template = ValidTemplate();
        template.Code = "";

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Template code is required."));
    }

    // ValidateTemplate – code whitespace
    [Test]
    public void ValidateTemplate_WhenCodeIsWhitespace_ShouldAddCodeError()
    {
        var template = ValidTemplate();
        template.Code = "   ";

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.Errors, Does.Contain("Template code is required."));
    }

    // ValidateTemplate – invalid notification type
    [Test]
    public void ValidateTemplate_WhenTypeIsInvalid_ShouldAddTypeError()
    {
        var template = ValidTemplate();
        template.Type = (NotificationType)999;

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Notification type is invalid."));
    }

    // ValidateTemplate – name empty
    [Test]
    public void ValidateTemplate_WhenNameIsEmpty_ShouldAddNameError()
    {
        var template = ValidTemplate();
        template.Name = "";

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Template name is required."));
    }

    // ValidateTemplate – email type, subject empty
    [Test]
    public void ValidateTemplate_WhenEmailTypeAndSubjectIsEmpty_ShouldAddSubjectError()
    {
        var template = ValidTemplate();
        template.Type = NotificationType.Email;
        template.SubjectTemplate = "";

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Subject template is required for email notifications."));
    }

    // ValidateTemplate – non-email type, subject empty is fine
    [Test]
    public void ValidateTemplate_WhenNonEmailTypeAndSubjectIsEmpty_ShouldNotAddSubjectError()
    {
        var template = ValidTemplate();
        template.Type = NotificationType.Push;
        template.SubjectTemplate = "";

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.Errors, Does.Not.Contain("Subject template is required for email notifications."));
    }

    // ValidateTemplate – body empty
    [Test]
    public void ValidateTemplate_WhenHtmlBodyIsEmpty_ShouldAddBodyError()
    {
        var template = ValidTemplate();
        template.HtmlBodyTemplate = "";

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Body template is required."));
    }

    // ValidateTemplate – version zero
    [Test]
    public void ValidateTemplate_WhenVersionIsZero_ShouldAddVersionError()
    {
        var template = ValidTemplate();
        template.Version = 0;

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Does.Contain("Template version must be greater than zero."));
    }

    // ValidateTemplate – version negative
    [Test]
    public void ValidateTemplate_WhenVersionIsNegative_ShouldAddVersionError()
    {
        var template = ValidTemplate();
        template.Version = -1;

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.Errors, Does.Contain("Template version must be greater than zero."));
    }

    // ValidateTemplate – multiple errors at once
    [Test]
    public void ValidateTemplate_WhenMultipleFieldsAreInvalid_ShouldReturnAllErrors()
    {
        var template = new NotificationTemplateDto
        {
            Code = "",
            Name = "",
            Type = NotificationType.Email,
            SubjectTemplate = "",
            HtmlBodyTemplate = "",
            Version = 0
        };

        var result = _sut.ValidateTemplate(template);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.GreaterThan(1));
    }

    private static NotificationTemplateDto ValidTemplate() =>
        new()
        {
            Code = "TMPL_CODE",
            Name = "Template Name",
            Type = NotificationType.Email,
            SubjectTemplate = "Subject",
            HtmlBodyTemplate = "<p>Body</p>",
            Version = 1
        };
}
