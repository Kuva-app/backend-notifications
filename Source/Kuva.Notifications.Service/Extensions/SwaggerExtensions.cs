using Microsoft.OpenApi;

namespace Kuva.Notifications.Service.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddKuvaSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Kuva.Notifications",
                Version = "v1",
                Description = "Transactional notifications microservice for the Kuva ecosystem."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
        });

        return services;
    }

    public static IApplicationBuilder UseKuvaSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kuva.Notifications v1");
        });

        return app;
    }
}
