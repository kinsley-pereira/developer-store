using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sales.Application.Interfaces;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Queries.GetSales
{
    public class GetSalesQueryHandler(IAppDbContext context, IMapper mapper) : IRequestHandler<GetSalesQuery, IEnumerable<SaleDto>>
    {
        private readonly IAppDbContext _db = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Sales
                .Include(x => x.Items)
                .AsNoTracking()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);

            var sales = await query.ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }
    }
}
