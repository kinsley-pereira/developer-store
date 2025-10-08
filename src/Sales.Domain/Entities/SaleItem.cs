namespace Sales.Domain.Entities
{
    public class SaleItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid SaleId { get; private set; }      // Chave estrangeira
        public Sale Sale { get; private set; } = null!; // Navegação de volta

        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal DiscountPercent { get; private set; } = 0m;
        public decimal DiscountAmount => Math.Round((UnitPrice * Quantity) * DiscountPercent / 100m, 2);
        public decimal TotalAmount => Math.Round((UnitPrice * Quantity) - DiscountAmount, 2);

        public bool IsCancelled { get; private set; } = false;

        protected SaleItem() { }

        public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            SetQuantity(quantity);
        }

        public void SetQuantity(int newQty)
        {
            ValidateQuantity(newQty);
            Quantity = newQty;
            RecalculateDiscount();
        }

        public void IncreaseQuantity(int delta)
        {
            SetQuantity(Quantity + delta);
        }

        private void ValidateQuantity(int q)
        {
            if (q <= 0) throw new ArgumentException("Quantity must be > 0");
            if (q > 20) throw new ArgumentException("Cannot sell more than 20 identical items");
        }

        private void RecalculateDiscount()
        {
            if (Quantity < 4) DiscountPercent = 0m;
            else if (Quantity < 10) DiscountPercent = 10m;
            else DiscountPercent = 20m;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }

        public void ChangeUnitPrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new ArgumentException("Unit price must be > 0");
            UnitPrice = newPrice;
        }
    }
}
