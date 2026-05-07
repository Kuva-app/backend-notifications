using Kuva.Email.Repository.Context;
using Kuva.Email.Repository.Interfaces;
using Kuva.Email.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kuva.Email.Repository.Extensions;

public static class RepositoryDependencyInjection
{
    public static IServiceCollection AddEmailRepository(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EmailDatabase");

        services.AddDbContext<EmailDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("Kuva.Email.EFMigrations"));
        });

        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEmailRequestRepository, EmailRequestRepository>();
        services.AddScoped<IEmailProviderRepository, EmailProviderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
