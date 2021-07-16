using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HizliSu.Catalog.Dto;
using HizliSu.Users.Dto;

namespace HizliSu.Orders.Dto
{

    [AutoMapFrom(typeof(Cart))]
    public class CartDto : FullAuditedEntityDto<long>
    {
        public long ProductId { get; set; }
        public ProductDto Product { get; set; }
        public long UserId { get; set; }
        public UserDto User { get; set; }
        public int Quantity { get; set; }
    }
}
