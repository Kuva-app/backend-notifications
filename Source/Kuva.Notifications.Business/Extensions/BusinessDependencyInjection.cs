using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Providers;
using Kuva.Notifications.Business.Services;
using Microsoft.Extensions.DependencyInjection; 

namespace Kuva.Notifications.Business.Extensions;

public static class BusinessDependencyInjection
{
    public static IServiceCollection AddNotificationBusiness(this IServiceCollection services)
    {
        services.AddScoped<INotificationDataAccess, NotificationDataAccess>();
        services.AddScoped<INotificationBusiness, NotificationBusiness>();
        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
        services.AddSingleton<INotificationValidationService, NotificationValidationService>();
        services.AddScoped<INotificationProviderFactory, NotificationProviderFactory>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<INotificationMetrics, NoopNotificationMetrics>();

        services.AddTransient<INotificationSender, FakeEmailSender>();
        services.AddTransient<INotificationSender, SmtpEmailSender>();
        services.AddHttpClient<INotificationSender, SendGridEmailSender>();

        return services;
    }
}
