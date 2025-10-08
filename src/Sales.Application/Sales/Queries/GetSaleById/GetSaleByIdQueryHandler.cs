using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sales.Application.Interfaces;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Queries.GetSaleById
{
    public class GetSaleByIdQueryHandler(IAppDbContext context, IMapper mapper) : IRequestHandler<GetSaleByIdQuery, SaleDto>
    {
        private readonly IAppDbContext _db = context;
        private readonly IMapper _mapper = mapper;

        public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            var sale = await _db.Sales
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (sale is null)
                throw new KeyNotFoundException($"Sale {request.Id} not found");

            return _mapper.Map<SaleDto>(sale);
        }
    }
}
