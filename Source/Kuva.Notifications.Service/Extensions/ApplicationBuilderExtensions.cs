using Kuva.Notifications.Service.Middlewares;

namespace Kuva.Notifications.Service.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseKuvaNotificationsPipeline(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseKuvaSwagger();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapKuvaObservability();

        return app;
    }
}
