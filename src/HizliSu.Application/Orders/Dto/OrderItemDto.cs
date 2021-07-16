using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HizliSu.Catalog.Dto;

namespace HizliSu.Orders.Dto
{
    [AutoMapFrom(typeof(OrderItem))]
    public class OrderItemDto : FullAuditedEntityDto<long>
    {
        public long OrderId { get; set; }
        public OrderDto Order { get; set; }

        public long ProductId { get; set; }
        public ProductDto Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
