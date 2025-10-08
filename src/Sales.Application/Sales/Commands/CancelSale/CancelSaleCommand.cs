using MediatR;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Commands.CancelSale
{
    public record CancelSaleCommand(Guid SaleId) : IRequest<SaleDto>;
}
