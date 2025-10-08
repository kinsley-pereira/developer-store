using MediatR;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Queries.GetSales
{
    public record GetSalesQuery(int Page = 1, int PageSize = 20) : IRequest<IEnumerable<SaleDto>>;
}
