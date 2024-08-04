using Microsoft.AspNetCore.Mvc;

namespace Dev2C2P.Services.Platform.Contracts.Transactions.Endpoints;

public record GetTransactionQueryParameters
{
    /// <summary>
    /// Filter by
    /// </summary>
    [FromQuery(Name = "filter")]
    public IDictionary<string, string> Filter { get; init; } = new Dictionary<string, string>();
}
