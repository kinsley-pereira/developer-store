using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sales.Application.Sales.Commands.CreateSale;
using Sales.Domain.Entities;
using Sales.Tests.Common;

namespace Sales.Tests.Application.Commands
{
    public class CreateSaleCommandHandlerTests : CommandHandlerTestBase
    {
        private readonly CreateSaleCommandHandler _handler;

        public CreateSaleCommandHandlerTests()
        {
            var mapper = CreateMapper();
            var logger = new Mock<ILogger<CreateSaleCommandHandler>>();
            _handler = new CreateSaleCommandHandler(DbMock.Object, mapper, logger.Object, EventPublisherMock.Object);
        }

        [Fact(DisplayName = "Deve criar uma venda com sucesso e publicar evento")]
        public async Task Handle_ShouldCreateSaleAndPublishEvent()
        {
            // Arrange
            var command = new CreateSaleCommand(
                CustomerId: Guid.NewGuid(),
                CustomerName: "Cliente Teste",
                BranchId: Guid.NewGuid(),
                BranchName: "Filial Central",
                SaleDate: DateTime.UtcNow,
                Items:
                [
                    new(Guid.NewGuid(), "Produto A", 2, 10m),
                    new(Guid.NewGuid(), "Produto B", 1, 25m)
                ]
            );

            var dbSetMock = new Mock<Microsoft.EntityFrameworkCore.DbSet<Sale>>();
            DbMock.Setup(x => x.Sales).Returns(dbSetMock.Object);

            DbMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalAmount.Should().BeGreaterThan(0);

            DbMock.Verify(x => x.Sales.Add(It.IsAny<Sale>()), Times.Once);
            DbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção se não houver itens na venda")]
        public async Task Handle_ShouldThrow_WhenNoItemsProvided()
        {
            // Arrange
            var command = new CreateSaleCommand(
                CustomerId: Guid.NewGuid(),
                CustomerName: "Cliente Teste",
                BranchId: Guid.NewGuid(),
                BranchName: "Filial Central",
                SaleDate: DateTime.UtcNow,
                Items: []
            );

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*item*");
        }
    }
}
