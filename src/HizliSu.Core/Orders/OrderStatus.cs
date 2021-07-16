using Abp.Domain.Entities.Auditing;

namespace HizliSu.Orders
{
    public class OrderStatus : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
