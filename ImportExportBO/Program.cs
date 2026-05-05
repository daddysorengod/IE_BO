using Application;
using Application.Security;
using NLog;
using NLog.Web;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

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

    // Configure app configuration
    builder.Configuration.AddJsonFile(ConfigFile, optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var jwtOptions = BuildJwtOptions(builder.Configuration);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
        });
    builder.Services.AddAuthorization();

    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhap access token JWT vao day."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

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

    app.UseAuthentication();
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

static JwtOptions BuildJwtOptions(IConfiguration configuration)
{
    var options = new JwtOptions
    {
        Issuer = configuration["Jwt:Issuer"]?.Trim() ?? string.Empty,
        Audience = configuration["Jwt:Audience"]?.Trim() ?? string.Empty,
        SecretKey = configuration["Jwt:SecretKey"]?.Trim() ?? string.Empty,
        AccessTokenExpirationMinutes = ParsePositiveInt(configuration["Jwt:AccessTokenExpirationMinutes"], 15),
        RefreshTokenExpirationDays = ParsePositiveInt(configuration["Jwt:RefreshTokenExpirationDays"], 7)
    };

    options.Validate();
    return options;
}

static int ParsePositiveInt(string? value, int defaultValue)
{
    return int.TryParse(value, out var parsed) && parsed > 0
        ? parsed
        : defaultValue;
}
