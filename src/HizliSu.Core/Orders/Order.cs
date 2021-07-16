using Abp.Domain.Entities.Auditing;
using HizliSu.Address;
using HizliSu.Authorization.Users;

namespace HizliSu.Orders
{
    public class Order : FullAuditedEntity<long>
    {
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long UserAddressId { get; set; }
        public virtual UserAddress UserAddress { get; set; }
        public long OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public string OrderNote { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
