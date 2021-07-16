using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Catalog;
using HizliSu.Enumerations;
using HizliSu.Orders.Dto;
using HizliSu.Orders.Request;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HizliSu.Orders
{
    [AbpAuthorize()]
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepository;
        private readonly IRepository<OrderItem, long> _orderItemRepository;
        private readonly IRepository<Product, long> _productRepository;
        private readonly IRepository<Cart, long> _cartRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrderAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<Order, long> orderRepository, IRepository<Cart, long> cartRepository, IRepository<Product, long> productRepository, IRepository<OrderItem, long> orderItemRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<ListResultDto<OrderDto>> GetOrderListAsync()
        {
            var userId = AbpSession.UserId;
            List<Order> orderList = await _orderRepository
                .GetAllIncluding(c => c.OrderStatus, b => b.UserAddress, c => c.UserAddress.City, e => e.UserAddress.District, f => f.UserAddress.Neighborhood)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreationTime).ToListAsync();
            return new ListResultDto<OrderDto>(ObjectMapper.Map<List<OrderDto>>(orderList));
        }

        public async Task<OrderDto> GetOrderDetailAsync(long orderId)
        {
            var userId = AbpSession.UserId;
            Order order = await _orderRepository
                .GetAllIncluding(c => c.OrderStatus, b => b.UserAddress, c => c.UserAddress.City,
                    e => e.UserAddress.District, f => f.UserAddress.Neighborhood)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);

            return ObjectMapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> SaveOrderCancelAsync(long orderId)
        {
            var userId = AbpSession.UserId;
            Order order = await _orderRepository.GetAllIncluding(c => c.OrderStatus).FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);
            if (order == null)
            {
                throw new UserFriendlyException("Sipariş bulunamadı!");
            }

            if (order.OrderStatusId != (int)EOrderStatus.OnayBekliyor)
            {
                throw new UserFriendlyException("Sipariş durumu artık değiştiremezsiniz! Sipariş durumunuz: " + order.OrderStatus.Name);
            }
            order.OrderStatusId = (int)EOrderStatus.IptalEdildi;
            await _orderRepository.UpdateAsync(order);

            order = await _orderRepository
                .GetAllIncluding(c => c.OrderStatus, b => b.UserAddress, c => c.UserAddress.City,
                    e => e.UserAddress.District, f => f.UserAddress.Neighborhood)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);

            return ObjectMapper.Map<OrderDto>(order);
        }

        public async Task<ListResultDto<OrderItemDto>> GetOrderItemListAsync(long orderId)
        {
            var userId = AbpSession.UserId;
            List<OrderItem> orderItemList = await _orderItemRepository
                .GetAllIncluding(c => c.Product, b => b.Product.Category, c => c.Product.Manufacturer, e => e.Product.Facility, f => f.Product.Image)
                .Where(x => x.OrderId == orderId)
                .OrderByDescending(x => x.CreationTime).ToListAsync();
            return new ListResultDto<OrderItemDto>(ObjectMapper.Map<List<OrderItemDto>>(orderItemList));
        }


        public async Task<OrderDto> SaveOrderAsync(OrderRequest request)
        {
            var userId = AbpSession.UserId;
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (!userId.HasValue)
            {
                throw new UserFriendlyException("Kullanıcı bulunamadı!");
            }

            var cartList = await _cartRepository.GetAllListAsync(x => x.UserId == userId);
            if (!cartList.Any())
            {
                throw new UserFriendlyException("Sepetinizde ürün bulunamadı!");
            }

            decimal totalPrice = 0;
            foreach (var item in cartList)
            {
                var product = await _productRepository.FirstOrDefaultAsync(item.ProductId);
                if (product == null)
                {
                    throw new UserFriendlyException(item.ProductId + " Id'li ürün bulunamadı!");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    throw new UserFriendlyException(product.Name +
                                                    " adlı ürüne ait yeterli stok bulunamadı! Lütfen ürünün adet miktarını düşürün veya sepetten çıkarınız!");
                }

                totalPrice += item.Quantity * product.Price;
            }

            var order = new Order()
            {
                OrderNote = request.OrderNote,
                OrderStatusId = (int)EOrderStatus.OnayBekliyor,
                UserId = userId.Value,
                UserAddressId = request.UserAddressId,
                TotalPrice = totalPrice
            };


            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                var orderId = await _orderRepository.InsertAndGetIdAsync(order);


                foreach (var item in cartList)
                {
                    var product = await _productRepository.FirstOrDefaultAsync(item.ProductId);
                    var orderItem = new OrderItem()
                    {
                        OrderId = orderId,
                        Price = product.Price,
                        ProductId = product.Id,
                        Quantity = item.Quantity
                    };
                    await _orderItemRepository.InsertAndGetIdAsync(orderItem);
                    await _cartRepository.DeleteAsync(item.Id);
                }

                await unitOfWork.CompleteAsync();
            }

            return ObjectMapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> DeleteOrderAsync(long orderId)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(orderId);
            if (order == null)
            {
                throw new UserFriendlyException("Kayıt bulunamadı!");
            }
            await _orderRepository.DeleteAsync(orderId);

            return ObjectMapper.Map<OrderDto>(order);
        }
    }
}
