using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Order.Request;
using HizliSu.Orders.Dto;

namespace HizliSu.Orders
{
    public interface ICartAppService : IApplicationService
    {
        Task<ListResultDto<CartDto>> GetCartListAsync();
        Task<ListResultDto<CartDto>> GetCartAllListAsync();
        Task<CartDto> SaveOrUpdateCartAsync(CartRequest request);
        Task<CartDto> DeleteCartAsync(long cartId);
    }
}
