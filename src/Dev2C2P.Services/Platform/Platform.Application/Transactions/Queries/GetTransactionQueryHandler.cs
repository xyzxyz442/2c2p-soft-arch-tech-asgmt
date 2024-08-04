using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Contracts.Transactions.Dtos;
using Dev2C2P.Services.Platform.Domain;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Dev2C2P.Services.Platform.Application.Transactions.Queries;

public class GetTransactionsQueryHandler
    : RequestHandlerBase<GetTransactionsQuery, IEnumerable<TransactionDto>>
{
    private readonly ITransactionService _service;

    public GetTransactionsQueryHandler(
        ITransactionService service,
        ILogger<RequestHandlerBase<GetTransactionsQuery, IEnumerable<TransactionDto>>> logger)
        : base(logger)
    {
        _service = service;
    }

    protected override async Task<ErrorOr<Result<IEnumerable<TransactionDto>>>> DoHandle(
        GetTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var dtos = new List<TransactionDto>();

        Expression<Func<Transaction, bool>> filter = e => true;

        var filterResult = BuildFilters(request, filter);
        if (filterResult.IsError)
            return filterResult.FirstError;

        var entities = await _service.GetAsync(
            filter,
            null,
            0,
            1,
            100
        );

        // TODO: should revise to dedicated mapper class
        return new Result<IEnumerable<TransactionDto>>(
            entities.Select(e => new TransactionDto(
                e.GetId(),
                $"{e.Amount} {e.Currency}",
                e.Status
            )).ToList()
        );
    }

    protected override string GetLogPrefix()
    {
        return nameof(GetTransactionsQueryHandler);
    }

    private ErrorOr<Expression<Func<Transaction, bool>>> BuildFilters(
        GetTransactionsQuery request,
        Expression<Func<Transaction, bool>> filter
    )
    {
        try
        {
            DateTime? from = null;
            DateTime? to = null;
            string? currencyCode = null;
            string? status = null;

            if (request.Currency is not null)
            {
                currencyCode = ISO._4217.CurrencyCodesResolver.Codes.FirstOrDefault(x => x.Code == request.Currency)?.Code;

                filter = e => e.Currency == currencyCode;
            }

            if (request.From is not null || request.From == string.Empty)
            {
                from = DateTime.Parse(request.From);

                filter = CombineFilters(filter, e => e.At >= from);
            }

            if (request.To is not null || request.To == string.Empty)
            {
                to = DateTime.Parse(request.To);

                filter = CombineFilters(filter, e => e.At <= to);
            }

            if ((request.Status is not null || request.Status == string.Empty) && new[] { "A", "R", "D" }.Contains(request.Status))
            {
                status = request.Status;

                filter = CombineFilters(filter, e => e.Status == status);
            }

            return filter;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return Error.Validation("FilterValidation", ex.Message);
        }
    }

    private Expression<Func<Transaction, bool>> CombineFilters(
        Expression<Func<Transaction, bool>> left,
        Expression<Func<Transaction, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(Transaction));
        var body = Expression.AndAlso(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
        return Expression.Lambda<Func<Transaction, bool>>(body, parameter);
    }
}
