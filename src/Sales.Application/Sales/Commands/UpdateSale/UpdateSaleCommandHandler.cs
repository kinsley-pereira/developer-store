using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;
using Sales.Contracts.Dto;

namespace Sales.Application.Sales.Commands.UpdateSale
{
    public class UpdateSaleCommandHandler(
        IAppDbContext db,
        IMapper mapper,
        ILogger<UpdateSaleCommandHandler> logger,
        IEventPublisher eventPublisher)
        : IRequestHandler<UpdateSaleCommand, SaleDto>
    {
        private readonly IAppDbContext _db = db;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UpdateSaleCommandHandler> _logger = logger;
        private readonly IEventPublisher _eventPublisher = eventPublisher;

        public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with id {request.Id} not found.");

            if (sale.IsCancelled)
                throw new InvalidOperationException("Cannot update a cancelled sale.");

            // Atualiza os dados básicos
            sale.UpdateCustomerInfo(request.CustomerId, request.CustomerName);
            sale.UpdateBranchInfo(request.BranchId, request.BranchName);
            sale.UpdateSaleDate(request.SaleDate);

            foreach (var item in sale.Items.ToList())
            {
                _db.SaleItems.Remove(item);
            }
            sale.ClearItems();
            foreach (var it in request.Items)
                sale.AddItem(it.ProductId, it.ProductName, it.Quantity, it.UnitPrice);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sale updated: {SaleId}", sale.Id);

            await _eventPublisher.PublishAsync(new Events.SaleUpdatedEvent(sale.Id, sale.SaleNumber, sale.TotalAmount));

            return _mapper.Map<SaleDto>(sale);
        }
    }
}
