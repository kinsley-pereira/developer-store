using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;
using Sales.Application.Sales.Events;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Commands.CancelSale
{
    public class CancelSaleCommandHandler(
        IAppDbContext db,
        IMapper mapper,
        ILogger<CancelSaleCommandHandler> logger,
        IEventPublisher eventPublisher)
        : IRequestHandler<CancelSaleCommand, SaleDto>
    {
        private readonly IAppDbContext _db = db;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CancelSaleCommandHandler> _logger = logger;
        private readonly IEventPublisher _eventPublisher = eventPublisher;

        public async Task<SaleDto> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with id {request.SaleId} not found.");

            sale.CancelSale();

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sale cancelled: {SaleId}", sale.Id);

            await _eventPublisher.PublishAsync(new SaleCancelledEvent(sale.Id, sale.SaleNumber));

            return _mapper.Map<SaleDto>(sale);
        }
    }
}
