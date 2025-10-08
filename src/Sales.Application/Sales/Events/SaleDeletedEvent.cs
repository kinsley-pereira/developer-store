namespace Sales.Application.Sales.Events
{
    public record SaleDeletedEvent (Guid SaleId, string SaleNumber, DateTime DeletedAt);
}
