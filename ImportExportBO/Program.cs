using BussinessLayer.Products;
using BussinessLayer.Products.Repositories;
using CommomLib;
using DataAccessLayer.Repositories.ProductImageRepositories;
using DataAccessLayer.Repositories.ProductRepositories;
using DataManagement.Helper;
using NLog;
using NLog.Web;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);


string ConfigLogFile = "NLogConfig/nlog.config";
string ConfigFile = "appsettings.json";

var logger = NLogBuilder.ConfigureNLog(ConfigLogFile).GetCurrentClassLogger();

try
{
    logger.Debug("Initializing main");
    // Configure for Windows Service
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "IE_service";
    });

    // Add services to the container.
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .WithExposedHeaders("Content-Disposition")
                              .WithExposedHeaders("Content-Type")
                              );
    });

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure app configuration
    builder.Configuration.AddJsonFile(ConfigFile, optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
    ConfigData.InitConfig(builder.Configuration);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddScoped<IDbManagement, DbManagement>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    // if (app.Environment.IsDevelopment())
    // {
        app.UseSwagger();
        app.UseSwaggerUI();
    // }
    app.UseCors("AllowAllOrigins");
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    logger.Error($"Error in Main program: {ex.ToString()}");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit
    NLog.LogManager.Shutdown();
}
