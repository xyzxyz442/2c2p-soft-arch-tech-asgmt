using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Contracts.Transactions;

namespace Dev2C2P.Services.Platform.Application.Transactions.Commands;

public record ImportTransactionCommand
    : RequestBase<bool>
{
    public ImportTransactionFileType Type { get; }

    public IEnumerable<ImportTransactionDto> Datas { get; }

    public ImportTransactionCommand(
        ImportTransactionFileType type,
        IEnumerable<ImportTransactionDto> datas)
    {
        Type = type;
        Datas = datas;
    }
}
