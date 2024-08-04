

using System.Reflection;
using Dev2C2P.Services.Platform.Application.Transactions.Commands;

namespace Dev2C2P.Services.Platform.Application.Transactions;

public class TransactionAutofacModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyOpenGenericTypes(typeof(ImportTransactionCommand).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();
    }
}
