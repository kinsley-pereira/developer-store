using MediatR;
using Sales.Contracts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Sales.Commands.UpdateSale
{
    public record UpdateSaleCommand(
        Guid Id,
        Guid CustomerId,
        string CustomerName,
        Guid BranchId,
        string BranchName,
        DateTime SaleDate,
        List<UpdateSaleItemDto> Items
    ) : IRequest<SaleDto>;

    public record UpdateSaleItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
}
