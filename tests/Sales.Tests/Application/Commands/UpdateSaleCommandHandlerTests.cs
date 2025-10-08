using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sales.Application.Interfaces;
using Sales.Application.Sales.Commands.UpdateSale;
using Sales.Contracts.Dto;
using Sales.Domain.Entities;
using Sales.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.Application.Commands
{
    public class UpdateSaleCommandHandlerTests : CommandHandlerTestBase
    {
        private readonly UpdateSaleCommandHandler _handler;

        public UpdateSaleCommandHandlerTests()
        {
            var mapper = CreateMapper();
            var logger = new Mock<ILogger<UpdateSaleCommandHandler>>();
            _handler = new UpdateSaleCommandHandler(DbMock.Object, mapper, logger.Object, EventPublisherMock.Object);
        }

        [Fact(DisplayName = "Deve atualizar uma venda existente e publicar evento")]
        public async Task Handle_ShouldUpdateSaleAndPublishEvent()
        {
            // Arrange
            var sale = new Sale(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Cliente Antigo",
                Guid.NewGuid(),
                "Filial A",
                DateTime.UtcNow.AddDays(-1),
                [new(Guid.NewGuid(), "Produto X", 1, 10)] 
            );

            var command = new UpdateSaleCommand(
                Id: sale.Id,
                CustomerId: Guid.NewGuid(),
                CustomerName: "Cliente Atualizado",
                BranchId: sale.BranchId,
                BranchName: "Filial Atualizada",
                SaleDate: sale.SaleDate,
                Items: [new(Guid.NewGuid(), "Produto Novo", 2, 20m)]
            );

            DbMock.Setup(x => x.Sales.FindAsync(sale.Id))
                .ReturnsAsync(sale);

            DbMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CustomerName.Should().Be("Cliente Atualizado");
            result.Items.Should().HaveCount(1);
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
            DbMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção se venda não for encontrada")]
        public async Task Handle_ShouldThrow_WhenSaleNotFound()
        {
            // Arrange
            var command = new UpdateSaleCommand(
                Id: Guid.NewGuid(),
                CustomerId: Guid.NewGuid(),
                CustomerName: "Cliente",
                BranchId: Guid.NewGuid(),
                BranchName: "Filial",
                SaleDate: DateTime.UtcNow,
                Items: [new(Guid.NewGuid(), "Produto A", 1, 10m)]
            );

            DbMock.Setup(x => x.Sales.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Sale?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
            EventPublisherMock.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Never);
        }
    }
}
