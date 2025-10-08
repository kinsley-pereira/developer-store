using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sales.Application.Sales.Commands.CancelSale;
using Sales.Domain.Entities;
using Sales.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.Application.Commands
{
    public class CancelSaleCommandHandlerTests : CommandHandlerTestBase
    {
        private readonly CancelSaleCommandHandler _handler;

        public CancelSaleCommandHandlerTests()
        {
            var mapper = CreateMapper();
            var logger = new Mock<ILogger<CancelSaleCommandHandler>>();
            _handler = new CancelSaleCommandHandler(DbMock.Object, mapper, logger.Object, EventPublisherMock.Object);
        }

        [Fact(DisplayName = "Deve cancelar uma venda existente e publicar evento")]
        public async Task Handle_ShouldCancelSaleAndPublishEvent()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid(), false);
            var command = new CancelSaleCommand(sale.Id);

            DbMock.Setup(x => x.Sales.FindAsync(sale.Id)).ReturnsAsync(sale);
            DbMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsCancelled.Should().BeTrue();
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
            DbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção se venda não for encontrada")]
        public async Task Handle_ShouldThrow_WhenSaleNotFound()
        {
            // Arrange
            var command = new CancelSaleCommand(Guid.NewGuid());
            DbMock.Setup(x => x.Sales.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Sale?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Never);
        }
    }
}
