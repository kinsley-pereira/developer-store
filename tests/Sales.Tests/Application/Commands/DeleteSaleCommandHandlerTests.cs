using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sales.Application.Sales.Commands.DeleteSale;
using Sales.Domain.Entities;
using Sales.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.Application.Commands
{
    ublic class DeleteSaleCommandHandlerTests : CommandHandlerTestBase
    {
        private readonly DeleteSaleCommandHandler _handler;

        public DeleteSaleCommandHandlerTests()
        {
            var logger = new Mock<ILogger<DeleteSaleCommandHandler>>();
            _handler = new DeleteSaleCommandHandler(DbMock.Object, logger.Object, EventPublisherMock.Object);
        }

        [Fact(DisplayName = "Deve deletar uma venda existente e publicar evento")]
        public async Task Handle_ShouldDeleteSaleAndPublishEvent()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var command = new DeleteSaleCommand(sale.Id);

            DbMock.Setup(x => x.Sales.FindAsync(sale.Id)).ReturnsAsync(sale);
            DbMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            DbMock.Verify(x => x.Sales.Remove(sale), Times.Once);
            DbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção se venda não for encontrada")]
        public async Task Handle_ShouldThrow_WhenSaleNotFound()
        {
            // Arrange
            var command = new DeleteSaleCommand(Guid.NewGuid());
            DbMock.Setup(x => x.Sales.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Sale?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Never);
        }
    }
}
