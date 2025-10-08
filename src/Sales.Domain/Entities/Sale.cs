namespace Sales.Domain.Entities
{
    public class Sale
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string SaleNumber { get; private set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public DateTime SaleDate { get; private set; } = DateTime.UtcNow;

        // External
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; } = string.Empty;
        public Guid BranchId { get; private set; }
        public string BranchName { get; private set; } = string.Empty;

        //private readonly List<SaleItem> _items = [];
        public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();

        public bool IsCancelled { get; private set; } = false;

        public decimal TotalAmount => Items.Sum(i => i.TotalAmount);

        protected Sale() { }

        public Sale(Guid customerId, string customerName, Guid branchId, string branchName, DateTime? saleDate = null)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            BranchId = branchId;
            BranchName = branchName;
            if (saleDate.HasValue) SaleDate = saleDate.Value;
        }

        public Sale(Guid id, Guid customerId, string customerName, Guid branchId, string branchName, DateTime saleDate, ICollection<SaleItem> items)
        {
            Id = id;
            CustomerId = customerId;
            CustomerName = customerName;
            BranchId = branchId;
            BranchName = branchName;
            SaleDate = saleDate;
            Items = items;
        }

        public Sale(Guid id, bool isCancelled)
        {
            Id = id;
            IsCancelled = isCancelled;
        }
        public Sale(Guid id)
        {
            Id = id;
        }

        public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            if (IsCancelled) throw new InvalidOperationException("Sale is cancelled");
            var existing = Items.FirstOrDefault(x => x.ProductId == productId && !x.IsCancelled);
            if (existing != null)
            {
                existing.IncreaseQuantity(quantity);
            }
            else
            {
                var item = new SaleItem(productId, productName, quantity, unitPrice);
                Items.Add(item);
            }
        }

        public void RemoveItem(Guid productId)
        {
            var item = Items.FirstOrDefault(x => x.ProductId == productId && !x.IsCancelled);
            if (item == null) throw new InvalidOperationException("Item not found");
            item.Cancel();
        }
        public void ClearItems()
        {
            if (Items.Count == 0) return;
            Items.Clear();
        }

        public void CancelSale()
        {
            if (IsCancelled) return;
            foreach (var it in Items) it.Cancel();
            IsCancelled = true;
        }

        public void UpdateCustomerInfo(Guid customerId, string customerName)
        {
            CustomerId = customerId;
            CustomerName = customerName;
        }

        public void UpdateBranchInfo(Guid branchId, string branchName)
        {
            BranchId = branchId;
            BranchName = branchName;
        }
        
        public void UpdateSaleDate(DateTime date)
        {
            SaleDate = date;
        }
    }
}
