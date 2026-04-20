using Application.Products.Repositories;
using Infrastructure.Persistence.Db;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new DatabaseOptions
        {
            ConnectionString = configuration["ConnectionString"]?.Trim() ?? string.Empty,
            DatabaseType = configuration["DatabaseType"]?.Trim()
        });

        services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        return services;
    }
}
