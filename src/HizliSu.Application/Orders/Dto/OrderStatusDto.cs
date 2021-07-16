using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace HizliSu.Orders.Dto
{
    [AutoMapFrom(typeof(OrderStatus))]
    public class OrderStatusDto : FullAuditedEntityDto<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
