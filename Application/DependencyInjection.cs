using Application.Allcodes;
using Application.Products;
using Application.Partners;
using Application.Contracts;
using Application.Packages;
using Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAllcodeService, AllcodeService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPartnerService, PartnerService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
