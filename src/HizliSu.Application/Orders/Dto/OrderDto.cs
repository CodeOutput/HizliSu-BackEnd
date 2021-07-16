using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HizliSu.Address;
using HizliSu.Address.Dto;
using HizliSu.Authorization.Users;
using HizliSu.Users.Dto;

namespace HizliSu.Orders.Dto
{
    [AutoMapFrom(typeof(Order))]
    public class OrderDto : FullAuditedEntityDto<long>
    {
        public long UserId { get; set; }
        public UserDto User { get; set; }
        public long UserAddressId { get; set; }
        public UserAddressDto UserAddress { get; set; }
        public long OrderStatusId { get; set; }
        public OrderStatusDto OrderStatus { get; set; }
        public string OrderNote { get; set; }

        public decimal TotalPrice { get; set; }

    }
}
