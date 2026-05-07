using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Providers;
using Kuva.Email.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kuva.Email.Business.Extensions;

public static class BusinessDependencyInjection
{
    public static IServiceCollection AddEmailBusiness(this IServiceCollection services)
    {
        services.AddScoped<IEmailBusiness, EmailBusiness>();
        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
        services.AddSingleton<IEmailValidationService, EmailValidationService>();
        services.AddScoped<IEmailProviderFactory, EmailProviderFactory>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IEmailMetrics, NoopEmailMetrics>();

        services.AddScoped<IEmailSender, FakeEmailSender>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddHttpClient<IEmailSender, SendGridEmailSender>();

        return services;
    }
}
