using Kuva.Email.Service.Middlewares;

namespace Kuva.Email.Service.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseKuvaEmailPipeline(this WebApplication app)
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
