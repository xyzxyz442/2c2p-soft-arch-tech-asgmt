using System.Reflection;
using Autofac.Core;
using Dev2C2P.Services.Platform.Application;
using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Infrastructure.Persistences;
using Dev2C2P.Services.Platform.Infrastructure.Services;

namespace Dev2C2P.Services.Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddPersistence();

        return services;
    }

    public static ContainerBuilder AddInfrastructureAutofac(this ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(TransactionRepository).GetTypeInfo().Assembly)
            .InstancePerLifetimeScope()
            .AsClosedTypesOf(typeof(IEFRepository<,,>));

        builder.RegisterAssemblyTypes(typeof(TransactionDbContext).GetTypeInfo().Assembly)
            .InstancePerLifetimeScope()
            .AsClosedTypesOf(typeof(DbContext))
            .WithParameter(new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(DbContextOptions<TransactionDbContext>),
                (pi, ctx) => new DbContextOptionsBuilder<TransactionDbContext>()
                    .UseNpgsql(ApplicationSettings.I.Database.ConnectionString)
                    .Options));

        return builder;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<TransactionDbContext>(options =>
        {
            options.UseNpgsql(
                ApplicationSettings.I.Database.ConnectionString,
                builder => builder.MigrationsAssembly(typeof(TransactionDbContext).Assembly.FullName));
        });

        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
