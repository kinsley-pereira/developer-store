namespace Sales.Application.Sales.Events
{
    public record SaleItemCancelledEvent(Guid SaleItemId, string ProductName);
}
