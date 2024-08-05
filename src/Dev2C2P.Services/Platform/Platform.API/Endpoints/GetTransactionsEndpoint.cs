
using Dev2C2P.Services.Platform.Application;
using Dev2C2P.Services.Platform.Application.Transactions.Queries;
using Dev2C2P.Services.Platform.Contracts.Transactions.Endpoints;
using ErrorOr;
using MediatR;

namespace Dev2C2P.Services.Platform.API.Endpoints;

[Route("transactions")]
[ApiExplorerSettings(GroupName = "transactions")]
public class GetTransactionsEndpoint
    : EndpointBaseAsync.WithRequest<GetTransactionQueryParameters>.WithResult<IActionResult>
{
    private readonly ILogger<GetTransactionsEndpoint> _logger;
    private readonly IOptionsMonitor<ApplicationSettings> _options;
    private readonly IMediator _mediator;

    public GetTransactionsEndpoint(
        ILogger<GetTransactionsEndpoint> logger,
        IOptionsMonitor<ApplicationSettings> options,
        IMediator mediator
    )
    {
        _mediator = mediator;
        _logger = logger;
        _options = options;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get transactions",
        Description = "Get transactions",
        OperationId = "Transactions.Get",
        Tags = new[] { "Transactions" }
    )]
    public override async Task<IActionResult> HandleAsync(
        [FromQuery] GetTransactionQueryParameters request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(request);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            _ => Ok(_),
            errors => BadRequest(new { errors = errors.Select(e => new { code = e.Code, message = e.Description }) })
        );
    }

    private GetTransactionsQuery BuildQuery(GetTransactionQueryParameters request)
    {
        var queryMap = new GetTransactionQueryMap();

        if (request.Filter.ContainsKey("currency")
            && request.Filter.TryGetValue("currency", out var currency))
        {
            queryMap.Currency = currency;
        }

        if (request.Filter.ContainsKey("from")
            && request.Filter.TryGetValue("from", out var from))
        {
            queryMap.From = from;
        }

        if (request.Filter.ContainsKey("to")
            && request.Filter.TryGetValue("to", out var to))
        {
            queryMap.To = to;
        }

        if (request.Filter.ContainsKey("status")
            && request.Filter.TryGetValue("status", out var status))
        {
            queryMap.Status = status;
        }

        return new GetTransactionsQuery(
            queryMap.Currency,
            queryMap.From,
            queryMap.To,
            queryMap.Status
        );
    }

    private record GetTransactionQueryMap
    {
        public string? Currency { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }

        public string? Status { get; set; }
    }
}
