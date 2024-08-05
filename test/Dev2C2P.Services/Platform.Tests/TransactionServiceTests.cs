using System.Linq.Expressions;
using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Domain;
using Dev2C2P.Services.Platform.Infrastructure.Services;

namespace Platform.Tests;

public class TransactionServiceTests
{
    [Fact]
    public async void GetAsync_ShouldReturnCorrentResult()
    {
        // Arrange
        var mockEntities = GetEntities();
        var mockRepository = new Mock<ITransactionRepository>();
        mockRepository.Setup(repository =>
            repository.GetAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(Task.FromResult(mockEntities));

        var service = new TransactionService(mockRepository.Object);

        // Act
        var result = await service.GetAsync(null, null, 0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockEntities.Count(), result.Count());

        for (int i = 0; i < mockEntities.Count(); i++)
        {
            Assert.Equal(mockEntities.ElementAt(i).TransactionId, result.ElementAt(i).TransactionId);
            Assert.Equal(mockEntities.ElementAt(i).Amount, result.ElementAt(i).Amount);
            Assert.Equal(mockEntities.ElementAt(i).Currency, result.ElementAt(i).Currency);
            Assert.Equal(mockEntities.ElementAt(i).Status, result.ElementAt(i).Status);
            Assert.Equal(mockEntities.ElementAt(i).At, result.ElementAt(i).At);
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
