using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Infrastructure.Services;

namespace Dev2C2P.Services.Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }

    // public static ContainerBuilder AddInfrastructureAutofac(this ContainerBuilder builder)
    // {

    // }
}
