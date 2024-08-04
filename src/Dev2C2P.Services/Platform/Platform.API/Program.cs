using System.Reflection;
using Dev2C2P.Services.Platform.API;
using Dev2C2P.Services.Platform.Application;

// Get configuration from appsettings.json
var configuration = GetConfiguration();

// Create logger
Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);
    var host = BuildWebHost(configuration, args);

    // TODO: apply database seeding (migration) if needed
    if (ApplicationSettings.I.IsSeedDatabase)
    {
        Log.Information("Applying migrations ({ApplicationContext})...", AppName);
        // MigrateDatabases(host);
    }

    Log.Information("Starting web host ({ApplicationContext})...", AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static IHost BuildWebHost(IConfiguration configuration, string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(builder =>
        {
            builder.CaptureStartupErrors(false);
            builder.UseStartup<Startup>();
            builder.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
            });
        })
        .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddConfiguration(configuration))
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .Build();

static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", AppName)
        .Enrich.WithDemystifiedStackTraces()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

static IConfiguration GetConfiguration()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

    builder.AddEnvironmentVariables()
        .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

    var config = builder.Build();

    // TODO: initialize your settings (e.g. UseVaults)

    return config;
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}
