using Dev2C2P.Services.Platform.Application.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace Dev2C2P.Services.Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }

    public static ContainerBuilder AddApplicationAutofac(this ContainerBuilder builder)
    {
        builder.RegisterModule<TransactionAutofacModule>();

        return builder;
    }
}
