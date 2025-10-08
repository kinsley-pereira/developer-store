using MediatR;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Queries.GetSaleById
{
    public record GetSaleByIdQuery(Guid Id) : IRequest<SaleDto>;
}
