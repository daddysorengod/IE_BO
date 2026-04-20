using Application;
using NLog;
using NLog.Web;
using Infrastructure;

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

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

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
