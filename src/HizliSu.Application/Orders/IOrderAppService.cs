using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Orders.Dto;
using HizliSu.Orders.Request;

namespace HizliSu.Orders
{
    public interface IOrderAppService : IApplicationService
    {
        Task<ListResultDto<OrderDto>> GetOrderListAsync();
        Task<OrderDto> GetOrderDetailAsync(long orderId);
        Task<OrderDto> SaveOrderCancelAsync(long orderId);
        Task<ListResultDto<OrderItemDto>> GetOrderItemListAsync(long orderId);
        Task<OrderDto> SaveOrderAsync(OrderRequest request);
        Task<OrderDto> DeleteOrderAsync(long id);
        
        
        //
        Task<ListResultDto<OrderDto>> GetOrderPagingListAsync(OrderPagingCo input);

        //
        Task<ListResultDto<OrderStatusDto>> GetOrderStatusListAsync();
    }
}
