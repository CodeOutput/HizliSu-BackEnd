using Abp.Domain.Entities.Auditing;
using HizliSu.Catalog;

namespace HizliSu.Orders
{
    public class OrderItem : FullAuditedEntity<long>
    {
        public long OrderId { get; set; }
        public virtual Order Order { get; set; }

        public long ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
