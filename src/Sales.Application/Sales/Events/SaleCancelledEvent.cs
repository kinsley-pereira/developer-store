namespace Sales.Application.Sales.Events
{
    public record SaleCancelledEvent(Guid SaleId, string SaleNumber);
}
