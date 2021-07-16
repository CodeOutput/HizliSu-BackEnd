using Abp.Domain.Entities.Auditing;
using HizliSu.Authorization.Users;
using HizliSu.Catalog;

namespace HizliSu.Orders
{
    public class Cart : FullAuditedEntity<long>
    {
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public int Quantity { get; set; }
    }
}
