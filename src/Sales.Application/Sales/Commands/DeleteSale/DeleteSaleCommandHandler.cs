using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;
using Sales.Application.Sales.Events;

namespace Sales.Application.Sales.Commands.DeleteSale
{
    public class DeleteSaleCommandHandler(
        IAppDbContext db,
        ILogger<DeleteSaleCommandHandler> logger,
        IEventPublisher eventPublisher)
        : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly IAppDbContext _db = db;
        private readonly ILogger<DeleteSaleCommandHandler> _logger = logger;
        private readonly IEventPublisher _eventPublisher = eventPublisher;

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with id {request.SaleId} not found.");

            _db.Sales.Remove(sale);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sale deleted: {SaleId}", sale.Id);

            await _eventPublisher.PublishAsync(new SaleDeletedEvent(sale.Id, sale.SaleNumber, DateTime.UtcNow));

            return Unit.Value;
        }
    }
}
