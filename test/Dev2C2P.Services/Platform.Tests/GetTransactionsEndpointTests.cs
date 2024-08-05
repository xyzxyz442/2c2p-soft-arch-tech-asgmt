using Dev2C2P.Services.Platform.API.Endpoints;
using Dev2C2P.Services.Platform.Application;
using Dev2C2P.Services.Platform.Application.Transactions.Queries;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Contracts.Transactions.Dtos;
using Dev2C2P.Services.Platform.Contracts.Transactions.Endpoints;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Platform.Tests;

public class GetTransactionsEndpointTests
{
    [Fact]
    public async void HandleAsync_ShouldReturnCorrectResult()
    {
        var dtos = GetDtos();

        // Arrange
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(mediator => mediator.Send(
                It.IsAny<GetTransactionsQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.FromResult(DtosToErrorOr()));
        var mockOptions = new Mock<IOptionsMonitor<ApplicationSettings>>();
        var mockLogger = new Mock<ILogger<GetTransactionsEndpoint>>();

        var queryParameters = new GetTransactionQueryParameters();

        var endpoint = new GetTransactionsEndpoint(
            mockLogger.Object,
            mockOptions.Object,
            mockMediator.Object
        );

        // Act
        var result = await endpoint.HandleAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<Result<IEnumerable<TransactionDto>>>(okResult.Value);

        Assert.NotNull(value.Data);
        Assert.Equal(dtos.Count(), value.Data.Count());

        for (int i = 0; i < dtos.Count(); i++)
        {
            Assert.Equal(dtos.ElementAt(i).Id, value.Data.ElementAt(i).Id);
            Assert.Equal(dtos.ElementAt(i).Payment, value.Data.ElementAt(i).Payment);
            Assert.Equal(dtos.ElementAt(i).Status, value.Data.ElementAt(i).Status);
        }
    }

    private IEnumerable<TransactionDto> GetDtos()
    {
        return
        [
            new("Txn0001", "10.00 USD", "A"),
            new("Txn0002", "20.00 USD", "R"),
            new("Txn0003", "30.00 USD", "D"),
        ];
    }

    private ErrorOr<Result<IEnumerable<TransactionDto>>> DtosToErrorOr()
    {
        return new Result<IEnumerable<TransactionDto>>(GetDtos());
    }

}
