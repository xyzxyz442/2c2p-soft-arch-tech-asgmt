using System.Linq.Expressions;
using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Application.Transactions.Queries;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Contracts.Transactions.Dtos;
using Dev2C2P.Services.Platform.Domain;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Platform.Tests;

public class GetTransactionsQueryHandlerTests
{
    [Fact]
    public async void HandleAsync_ShouldReturnCorrectResult()
    {
        // Arrange
        var mockEntities = GetEntities();
        var mockService = new Mock<ITransactionService>();
        mockService.Setup(service =>
            service.GetAsync(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(Task.FromResult(mockEntities));
        var mockLogger = new Mock<ILogger<GetTransactionsQueryHandler>>();

        var handler = new GetTransactionsQueryHandler(
            mockService.Object,
            mockLogger.Object);

        // Act
        var query = new GetTransactionsQuery(null, null, null, null);
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var value = Assert.IsType<ErrorOr<Result<IEnumerable<TransactionDto>>>>(result);
        Assert.NotNull(value.Value);

        var valueResult = Assert.IsType<Result<IEnumerable<TransactionDto>>>(value.Value);
        Assert.NotNull(valueResult.Data);

        var valueData = Assert.IsAssignableFrom<IEnumerable<TransactionDto>>(valueResult.Data);
        Assert.Equal(mockEntities.Count(), valueData.Count());

        for (int i = 0; i < mockEntities.Count(); i++)
        {
            Assert.Equal(mockEntities.ElementAt(i).TransactionId, valueData.ElementAt(i).Id);
            var paymentSplits = valueData.ElementAt(i).Payment.Split(" ");

            Assert.Equal(2, paymentSplits.Length);

            var amount = Assert.IsAssignableFrom<decimal>(Convert.ToDecimal(paymentSplits[0]));
            var currency = Assert.IsAssignableFrom<string>(paymentSplits[1]);

            Assert.Equal(mockEntities.ElementAt(i).Amount, amount);
            Assert.Equal(mockEntities.ElementAt(i).Currency, currency);
            Assert.Equal(mockEntities.ElementAt(i).Status, valueData.ElementAt(i).Status);
        }
    }

    private IEnumerable<Transaction> GetEntities()
    {
        return
        [
            Transaction.Create("Txn0001", 10.0m, "USD", "A", new DateTime(2024, 1, 1, 1, 0, 0)),
            Transaction.Create("Txn0002", 20.0m, "USD", "R", new DateTime(2024, 1, 2, 1, 0, 0)),
            Transaction.Create("Txn0003", 30.0m, "USD", "D", new DateTime(2024, 1, 3, 1, 0, 0)),
        ];
    }
}
