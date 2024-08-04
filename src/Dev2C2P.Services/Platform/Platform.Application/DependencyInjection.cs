using Dev2C2P.Services.Platform.Application.Behaviors;
using Dev2C2P.Services.Platform.Application.Transactions;

namespace Dev2C2P.Services.Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Add Validation
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    public static ContainerBuilder AddApplicationAutofac(this ContainerBuilder builder)
    {
        builder.RegisterModule<TransactionAutofacModule>();

        return builder;
    }
}
