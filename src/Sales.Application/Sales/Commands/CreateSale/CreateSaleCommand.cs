using MediatR;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Commands.CreateSale
{
    public record CreateSaleCommand(
        Guid CustomerId,
        string CustomerName,
        Guid BranchId,
        string BranchName,
        DateTime? SaleDate,
        List<CreateSaleItemDto> Items
    ) : IRequest<SaleDto>;

    public record CreateSaleItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
}
