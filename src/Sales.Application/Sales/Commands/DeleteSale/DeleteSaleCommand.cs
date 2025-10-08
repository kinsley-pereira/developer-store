using MediatR;

namespace Sales.Application.Sales.Commands.DeleteSale
{
    public record DeleteSaleCommand(Guid SaleId) : IRequest<Unit>;
}
