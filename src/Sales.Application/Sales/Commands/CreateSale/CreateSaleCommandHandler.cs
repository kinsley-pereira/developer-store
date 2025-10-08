using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;
using Sales.Application.Sales.Events;
using Sales.Contracts.Dto;
using Sales.Domain.Entities;

namespace Sales.Application.Sales.Commands.CreateSale
{
    public class CreateSaleCommandHandler(
        IAppDbContext db,
        IMapper mapper,
        ILogger<CreateSaleCommandHandler> logger,
        IEventPublisher eventPublisher)
        : IRequestHandler<CreateSaleCommand, SaleDto>
    {
        private readonly IAppDbContext _db = db;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CreateSaleCommandHandler> _logger = logger;
        private readonly IEventPublisher _eventPublisher = eventPublisher;

        public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = new Sale(request.CustomerId, request.CustomerName, request.BranchId, request.BranchName, request.SaleDate);

            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Cannot create a sale with no items.");

            foreach (var it in request.Items)
                sale.AddItem(it.ProductId, it.ProductName, it.Quantity, it.UnitPrice);

            _db.Sales.Add(sale);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sale created: {SaleId}", sale.Id);

            await _eventPublisher.PublishAsync(new SaleCreatedEvent(sale.Id, sale.SaleNumber, sale.SaleDate, sale.TotalAmount));

            return _mapper.Map<SaleDto>(sale);
        }
    }
}
