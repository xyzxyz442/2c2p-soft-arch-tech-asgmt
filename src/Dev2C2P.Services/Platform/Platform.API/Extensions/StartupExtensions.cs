using Microsoft.Extensions.Options;

namespace Dev2C2P.Services.Platform.API.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomConfiguration(
        this IServiceCollection services, IConfiguration configuration)
    {
        ApplicationSettings settings = ApplicationSettings.I;

        configuration.Bind("Database", settings.Database);

        services.AddSingleton<IOptionsMonitor<ApplicationSettings>, OptionsMonitor<ApplicationSettings>>();
        services.Configure<ApplicationSettings>(configuration);

        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition"));
        });

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddCustomStorages(this IServiceCollection services)
    {
        var dirPath = "tmp";

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        return services;
    }
}
