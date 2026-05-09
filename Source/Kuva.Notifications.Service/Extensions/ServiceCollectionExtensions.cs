using Kuva.Notifications.Business.Extensions;
using Kuva.Notifications.Repository.Extensions;
using Kuva.Notifications.Service.Filters;
using Kuva.Notifications.Service.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace Kuva.Notifications.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKuvaNotificationsService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.Configure<NotificationOptions>(configuration.GetSection("Notifications"));
        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        services.Configure<SendGridOptions>(configuration.GetSection("SendGrid"));
        services.Configure<KeyVaultOptions>(configuration.GetSection("KeyVault"));

        services.AddControllers(options => options.Filters.Add<ValidateModelStateFilter>())
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(context.ModelState);
            });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("notification-send", limiter =>
            {
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.PermitLimit = 120;
                limiter.QueueLimit = 20;
            });
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Jwt:Authority"];
                options.Audience = configuration["Jwt:Audience"];
                options.RequireHttpsMetadata = !environment.IsDevelopment();
            });

        services.AddAuthorization();
        services.AddKuvaSwagger();
        services.AddNotificationsRepository(configuration);
        services.AddNotificationBusiness();
        services.AddKuvaObservability();

        return services;
    }
}
