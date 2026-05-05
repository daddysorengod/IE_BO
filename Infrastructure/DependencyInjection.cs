using Application.Allcodes.Repositories;
using Application.Contracts.Repositories;
using Application.Packages.Repositories;
using Application.Partners.Repositories;
using Application.Products.Repositories;
using Application.Security;
using Application.Users.Repositories;
using Infrastructure.Persistence.Db;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions
        {
            Issuer = configuration["Jwt:Issuer"]?.Trim() ?? string.Empty,
            Audience = configuration["Jwt:Audience"]?.Trim() ?? string.Empty,
            SecretKey = configuration["Jwt:SecretKey"]?.Trim() ?? string.Empty,
            AccessTokenExpirationMinutes = ParseInt(configuration["Jwt:AccessTokenExpirationMinutes"], 15),
            RefreshTokenExpirationDays = ParseInt(configuration["Jwt:RefreshTokenExpirationDays"], 7)
        };
        jwtOptions.Validate();
        var superUserOptions = new SuperUserOptions
        {
            SupperUser = configuration["SuperAdmin:supperuser"]?.Trim() ?? string.Empty,
            SupperPassword = configuration["SuperAdmin:supperpassword"]?.Trim() ?? string.Empty,
            UserId = ParseLong(configuration["SuperAdmin:userid"], 999999),
            FullName = configuration["SuperAdmin:fullname"]?.Trim() ?? "Supper Admin",
            Role = configuration["SuperAdmin:role"]?.Trim() ?? "SuperAdmin",
            ImageUrl = configuration["SuperAdmin:imageurl"]?.Trim()
        };
        superUserOptions.Validate();

        services.AddSingleton(new DatabaseOptions
        {
            ConnectionString = configuration["ConnectionString"]?.Trim() ?? string.Empty,
            DatabaseType = configuration["DatabaseType"]?.Trim()
        });

        services.AddSingleton(jwtOptions);
        services.AddSingleton(superUserOptions);
        services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddScoped<IAllcodeRepository, AllcodeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IPartnerRepository, PartnerRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IPackageRepository, PackageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<ISuperUserSessionStore, InMemorySuperUserSessionStore>();
        return services;
    }

    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var result) && result > 0
            ? result
            : defaultValue;
    }

    private static long ParseLong(string? value, long defaultValue)
    {
        return long.TryParse(value, out var result) && result > 0
            ? result
            : defaultValue;
    }
}
