using Kuva.Email.Business.Extensions;
using Kuva.Email.Repository.Extensions;
using Kuva.Email.Service.Filters;
using Kuva.Email.Service.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace Kuva.Email.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKuvaEmailService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.Configure<EmailOptions>(configuration.GetSection("Email"));
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
            options.AddFixedWindowLimiter("email-send", limiter =>
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
        services.AddEmailRepository(configuration);
        BusinessDependencyInjection.AddEmailBusiness();
        services.AddKuvaObservability();

        return services;
    }
}
