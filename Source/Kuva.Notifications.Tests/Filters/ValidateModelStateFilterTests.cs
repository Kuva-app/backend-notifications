using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Service.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Filters;

[TestFixture]
public class ValidateModelStateFilterTests
{
    private ValidateModelStateFilter _filter = null!;

    [SetUp]
    public void SetUp()
    {
        _filter = new ValidateModelStateFilter();
    }

    private static ActionExecutingContext CreateActionExecutingContext(ModelStateDictionary modelState, string traceIdentifier = "trace-123")
    {
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(h => h.TraceIdentifier).Returns(traceIdentifier);

        var actionContext = new ActionContext(
            httpContext.Object,
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            modelState);

        return new ActionExecutingContext(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new object());
    }

    private static ActionExecutedContext CreateActionExecutedContext()
    {
        var httpContext = new Mock<HttpContext>();
        var actionContext = new ActionContext(
            httpContext.Object,
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            new ModelStateDictionary());

        return new ActionExecutedContext(actionContext, [], new object());
    }

    [Test]
    public void OnActionExecuting_WhenModelStateIsValid_DoesNotSetResult()
    {
        var modelState = new ModelStateDictionary();
        var context = CreateActionExecutingContext(modelState);

        _filter.OnActionExecuting(context);

        Assert.That(context.Result, Is.Null);
    }

    [Test]
    public void OnActionExecuting_WhenModelStateIsInvalid_SetsBadRequestResult()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field1", "Field1 is required.");
        var context = CreateActionExecutingContext(modelState);

        _filter.OnActionExecuting(context);

        Assert.That(context.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void OnActionExecuting_WhenModelStateIsInvalid_ReturnsErrorResponseDto()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field1", "Field1 is required.");
        var context = CreateActionExecutingContext(modelState, "trace-abc");

        _filter.OnActionExecuting(context);

        var result = context.Result as BadRequestObjectResult;
        Assert.That(result, Is.Not.Null);
        var dto = result!.Value as ErrorResponseDto;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto.Error, Is.EqualTo("InvalidRequest"));
        Assert.That(dto.Message, Is.EqualTo("The request payload is invalid."));
        Assert.That(dto.CorrelationId, Is.EqualTo("trace-abc"));
    }

    [Test]
    public void OnActionExecuting_WhenModelStateIsInvalid_IncludesErrorDetails()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field1", "Field1 is required.");
        modelState.AddModelError("Field2", "Field2 must be positive.");
        var context = CreateActionExecutingContext(modelState);

        _filter.OnActionExecuting(context);

        var result = context.Result as BadRequestObjectResult;
        Assert.That(result, Is.Not.Null);
        var dto = result!.Value as ErrorResponseDto;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Details, Does.Contain("Field1: Field1 is required."));
        Assert.That(dto!.Details, Does.Contain("Field2: Field2 must be positive."));
    }

    [Test]
    public void OnActionExecuting_WhenModelStateHasMultipleErrorsOnSameKey_IncludesAllErrors()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field1", "Error A.");
        modelState.AddModelError("Field1", "Error B.");
        var context = CreateActionExecutingContext(modelState);

        _filter.OnActionExecuting(context);

        var result = context.Result as BadRequestObjectResult;
        Assert.That(result, Is.Not.Null);
        var dto = result!.Value as ErrorResponseDto;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Details, Does.Contain("Field1: Error A."));
        Assert.That(dto!.Details, Does.Contain("Field1: Error B."));
    }

    [Test]
    public void OnActionExecuted_DoesNotThrow()
    {
        var context = CreateActionExecutedContext();

        var act = () => _filter.OnActionExecuted(context);

        Assert.DoesNotThrow(() => act());
    }
}
