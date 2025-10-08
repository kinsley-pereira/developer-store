namespace Sales.Application.Sales.Events
{
    public record SaleUpdatedEvent(Guid SaleId, string SaleNumber, decimal TotalAmount);
}
