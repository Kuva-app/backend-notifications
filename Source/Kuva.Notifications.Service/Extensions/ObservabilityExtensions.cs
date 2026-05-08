using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Kuva.Notifications.Service.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddKuvaObservability(this IServiceCollection services)
    {
        services.AddSingleton<INotificationMetrics, PrometheusNotificationMetrics>();
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddCheck<SqlServerHealthCheck>("sqlserver", tags: ["ready"]);

        return services;
    }

    public static IEndpointRouteBuilder MapKuvaObservability(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health");
        endpoints.MapHealthChecks("/health/live", new()
        {
            Predicate = check => check.Tags.Contains("live")
        });
        endpoints.MapHealthChecks("/health/ready", new()
        {
            Predicate = check => check.Tags.Contains("ready")
        });
        endpoints.MapMetrics("/metrics");
        return endpoints;
    }
}

public sealed class PrometheusNotificationMetrics : INotificationMetrics
{
    private static readonly Counter RequestsTotal = Metrics.CreateCounter("kuva_notifications_requests_total", "Total notification requests received.");
    private static readonly Counter SentTotal = Metrics.CreateCounter("kuva_notifications_sent_total", "Total notifications sent successfully.");
    private static readonly Counter FailedTotal = Metrics.CreateCounter("kuva_notifications_failed_total", "Total failed notification requests.");
    private static readonly Counter TemplateNotFoundTotal = Metrics.CreateCounter("kuva_notifications_template_not_found_total", "Total notification requests with template not found.");
    private static readonly Counter InvalidVariablesTotal = Metrics.CreateCounter("kuva_notifications_invalid_variables_total", "Total notification requests with invalid variables.");
    private static readonly Counter ProviderFailuresTotal = Metrics.CreateCounter("kuva_notifications_provider_failures_total", "Total provider failures.", "provider");
    private static readonly Histogram SendDuration = Metrics.CreateHistogram(
        "kuva_notifications_send_duration_seconds",
        "Notification send duration in seconds.",
        new HistogramConfiguration
        {
            LabelNames = ["status", "provider"],
            Buckets = Histogram.ExponentialBuckets(0.01, 2, 12)
        });

    public void RequestReceived() => RequestsTotal.Inc();

    public void SendCompleted(NotificationRequestStatus status, string providerName, double durationSeconds)
    {
        switch (status)
        {
            case NotificationRequestStatus.Sent:
                SentTotal.Inc();
                break;
            case NotificationRequestStatus.TemplateNotFound:
                TemplateNotFoundTotal.Inc();
                break;
            case NotificationRequestStatus.InvalidVariables:
                InvalidVariablesTotal.Inc();
                break;
            case NotificationRequestStatus.Failed:
                FailedTotal.Inc();
                break;
        }

        SendDuration.WithLabels(status.ToString(), providerName).Observe(durationSeconds);
    }

    public void ProviderFailure(string providerName) => ProviderFailuresTotal.WithLabels(providerName).Inc();
}

public sealed class SqlServerHealthCheck(NotificationsDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("SQL Server is not reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Server health check failed.", ex);
        }
    }
}
