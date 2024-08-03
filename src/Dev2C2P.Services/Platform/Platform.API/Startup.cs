using Dev2C2P.Services.Platform.API.Extensions;
using Microsoft.Extensions.Options;

namespace Dev2C2P.Services.Platform.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddCustomConfiguration(Configuration)
            .AddCustomMvc()
            .AddCustomHealthChecks()
            .AddCustomSwagger()
            .AddCustomStorages();

        // services.AddApplication();
        // services.AddInfrastructure();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        var pathBase = Configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase))
        {
            loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
            app.UsePathBase(pathBase);
        }

        app.UseRouting();
        app.UseCors("CorsPolicy");

        // Must be after UseRouting() and before UseEndpoints()
        ConfigureAuth(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });

        ConfigureSecurity(app);
        ConfigureSwagger(app, env);
    }

    /// <summary>
    /// Configure dependency injection with AutoFac
    /// </summary>
    /// <param name="builder"></param>
    public void ConfigureContainer(ContainerBuilder builder)
    {
        // builder.AddApplicationAutofac();
        // builder.AddInfrastructureAutofac();
    }

    /// <summary>
    /// Configure authentication and authorization middlewares
    /// </summary>
    /// <param name="app"></param>
    private void ConfigureAuth(IApplicationBuilder app)
    {
        // app.UseAuthentication(); // must be after UseRouting()
        // app.UseAuthorization(); // must be after UseAuthentication()
    }

    /// <summary>
    /// Configure security middleware (recommended from NWebSec)
    /// </summary>
    /// <param name="app"></param>
    private void ConfigureSecurity(IApplicationBuilder app)
    {
        app.UseXXssProtection(options => options.EnabledWithBlockMode());
        app.UseXfo(options => options.Deny());
        app.UseRedirectValidation(options =>
        {
            //options.AllowedDestinations("https://www.facebook.com/v4.0/dialog/oauth/");
        });

        /*app.UseCsp(opts => opts
          .BlockAllMixedContent()
          .StyleSources(s => s.Self())
          .StyleSources(s => s.UnsafeInline())
          .FontSources(s => s.Self())
          .FormActions(s => s.Self())
          .FrameAncestors(s => s.Self())
          .ImageSources(s => s.Self())
          .ScriptSources(s => s.Self()));*/
    }

    private void ConfigureSwagger(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) return;

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
