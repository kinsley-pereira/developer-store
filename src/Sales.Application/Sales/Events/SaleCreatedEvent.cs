namespace Sales.Application.Sales.Events
{
    public record SaleCreatedEvent(Guid SaleId, string SaleNumber, DateTime SaleDate, decimal TotalAmount);
}
