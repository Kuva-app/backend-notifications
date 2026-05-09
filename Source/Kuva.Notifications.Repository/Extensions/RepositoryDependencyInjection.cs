using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Repository.Interfaces;
using Kuva.Notifications.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kuva.Notifications.Repository.Extensions;

public static class RepositoryDependencyInjection
{
    public static IServiceCollection AddNotificationsRepository(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NotificationsDatabase");

        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("Kuva.Notifications.EFMigrations"));
        });

        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<INotificationRequestRepository, NotificationRequestRepository>();
        services.AddScoped<INotificationProviderRepository, NotificationProviderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
