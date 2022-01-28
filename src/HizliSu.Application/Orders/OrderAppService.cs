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
using Abp.AutoMapper;
using HizliSu.Authorization;

namespace HizliSu.Orders
{
    [AbpAuthorize()]
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepository;
        private readonly IRepository<OrderStatus, long> _orderStatusRepository;
        private readonly IRepository<OrderItem, long> _orderItemRepository;
        private readonly IRepository<Product, long> _productRepository;
        private readonly IRepository<Cart, long> _cartRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrderAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<Order, long> orderRepository,
            IRepository<Cart, long> cartRepository, IRepository<Product, long> productRepository,
            IRepository<OrderItem, long> orderItemRepository, IRepository<OrderStatus, long> orderStatusRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<ListResultDto<OrderDto>> GetOrderListAsync()
        {
            var userId = AbpSession.UserId;
            List<Order> orderList = await _orderRepository
                .GetAllIncluding(c => c.OrderStatus, b => b.UserAddress, c => c.UserAddress.City,
                    e => e.UserAddress.District, f => f.UserAddress.Neighborhood)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreationTime).ToListAsync();
            return new ListResultDto<OrderDto>(ObjectMapper.Map<List<OrderDto>>(orderList));
        }

        [AbpAuthorize(PermissionNames.Pages_Orders)]
        public async Task<ListResultDto<OrderDto>> GetOrderPagingListAsync(OrderPagingCo input)
        {
            var query = _orderRepository
                .GetAllIncluding(
                    c => c.OrderStatus,
                    b => b.UserAddress, c => c.UserAddress.City,
                    e => e.UserAddress.District,
                    f => f.UserAddress.Neighborhood,
                    g => g.User)
                .OrderByDescending(x => x.CreationTime).AsQueryable();


            if (!string.IsNullOrEmpty(input.OrderNote))
            {
                query = query.Where(x =>
                    x.OrderNote != null &&
                    EF.Functions.Like(x.OrderNote.ToLower(), "%" + input.OrderNote.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(input.Name))
            {
                query = query.Where(x => EF.Functions.Like(x.User.Name.ToLower(), "%" + input.Name.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(input.Surname))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.User.Surname.ToLower(), "%" + input.Surname.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(input.DistrictName))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.UserAddress.District.Name.ToLower(), "%" + input.DistrictName.ToLower() + "%"));
            }

            if (!string.IsNullOrEmpty(input.NeighborhoodName))
            {
                query = query.Where(x => EF.Functions.Like(x.UserAddress.Neighborhood.Name.ToLower(),
                    "%" + input.NeighborhoodName.ToLower() + "%"));
            }


            if (input.Id.HasValue)
            {
                query = query.Where(x => x.Id == input.Id.Value);
            }


            if (input.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == input.UserId.Value);
            }

            if (input.OrderStatusId.HasValue)
            {
                query = query.Where(x => x.OrderStatusId == input.OrderStatusId.Value);
            }

            if (input.CreationTime.HasValue)
            {
                query = query.Where(x => x.CreationTime.Date == input.CreationTime.Value.Date);
            }


            input.PageNo = input.PageNo < 0 ? 0 : input.PageNo;
            var itemsPerPage = input.ResultCount < 1 ? 20 : input.ResultCount;

            //Total number obtained
            var queryCount = await query.CountAsync();
            //Default Paging Method
            var queryList = await query.Skip(input.PageNo * itemsPerPage).Take(itemsPerPage).ToListAsync();

            return new PagedResultDto<OrderDto>(queryCount, queryList.MapTo<List<OrderDto>>());
        }


        [AbpAuthorize(PermissionNames.Pages_Orders)]
        public async Task<OrderDto> GetOrderDetailForAdminAsync(long orderId)
        {
            var order = await _orderRepository
                .GetAllIncluding(
                    c => c.OrderStatus,
                    b => b.UserAddress, c => c.UserAddress.City,
                    e => e.UserAddress.District,
                    f => f.UserAddress.Neighborhood,
                    g => g.User).FirstOrDefaultAsync(x => x.Id == orderId);

            return ObjectMapper.Map<OrderDto>(order);

        }

        [AbpAuthorize(PermissionNames.Pages_Orders)]
        public async Task<ListResultDto<OrderItemDto>> GetOrderItemListForAdminAsync(long orderId)
        {
            List<OrderItem> orderItemList = await _orderItemRepository
                .GetAllIncluding(c => c.Product, b => b.Product.Category, c => c.Product.Manufacturer,
                    e => e.Product.Facility, f => f.Product.Image)
                .Where(x => x.OrderId == orderId)
                .OrderByDescending(x => x.CreationTime).ToListAsync();
            return new ListResultDto<OrderItemDto>(ObjectMapper.Map<List<OrderItemDto>>(orderItemList));
        }

        [AbpAuthorize(PermissionNames.Pages_Orders)]
        public async Task<OrderDto> UpdateOrderStatusForAdminAsync(long orderId, OrderDto input)
        {
            Order order = await _orderRepository.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
            {
                throw new UserFriendlyException("Sipariş bulunamadı!");
            }
            order.OrderStatusId = input.OrderStatusId;
            await _orderRepository.UpdateAsync(order);

            return ObjectMapper.Map<OrderDto>(order);
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
                .Where(x => x.OrderId == orderId && x.Order.UserId == userId)
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

        // orderstatus
        public async Task<ListResultDto<OrderStatusDto>> GetOrderStatusListAsync()
        { 
            List<OrderStatus> orderStatusList = (await _orderStatusRepository.GetAllListAsync()).OrderBy(x => x.Id).ToList();
            return new ListResultDto<OrderStatusDto>(ObjectMapper.Map<List<OrderStatusDto>>(orderStatusList));
        }
    }
}
