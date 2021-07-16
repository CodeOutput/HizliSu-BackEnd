using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Catalog;
using HizliSu.Order.Request;
using HizliSu.Orders.Dto;
using Microsoft.EntityFrameworkCore;

namespace HizliSu.Orders
{
    [AbpAuthorize()]
    public class CartAppService : ApplicationService, ICartAppService
    {
        private readonly IRepository<Cart, long> _cartRepository;
        private readonly IRepository<Product, long> _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CartAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<Product, long> productRepository, IRepository<Cart, long> cartRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
        }
  
        public async Task<ListResultDto<CartDto>> GetCartListAsync()
        {
            var userId = AbpSession.UserId;
            ;
            List<Cart> productList = await _cartRepository
                .GetAllIncluding(c => c.Product)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Product.Name).ToListAsync();
            return new ListResultDto<CartDto>(ObjectMapper.Map<List<CartDto>>(productList));
        }
          
        public async Task<ListResultDto<CartDto>> GetCartAllListAsync()
        {
            var userId = AbpSession.UserId;
            ;
            List<Cart> productList = await _cartRepository
                .GetAllIncluding(c => c.Product,c=>c.Product.Image, c =>c.Product.Category,c=>c.Product.Manufacturer)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Product.Name).ToListAsync();
            return new ListResultDto<CartDto>(ObjectMapper.Map<List<CartDto>>(productList));
        }

        public async Task<CartDto> SaveOrUpdateCartAsync(CartRequest request)
        {
            var userId = AbpSession.UserId;
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (!userId.HasValue)
            {
                throw new UserFriendlyException("Kullanıcı bulunamadı!");
            }

            var cart = await _cartRepository.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.ProductId == request.ProductId);
            if (cart == null)
            {
                cart = new Cart();
                cart.UserId = userId.Value;
                cart.ProductId = request.ProductId;
            }
            else if (request.Quantity < 1)
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    await _cartRepository.DeleteAsync(cart.Id);
                    await unitOfWork.CompleteAsync();
                }

                return ObjectMapper.Map<CartDto>(cart);
            }

            cart.Quantity = request.Quantity;


            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                var cartId = await _cartRepository.InsertOrUpdateAndGetIdAsync(cart);
                cart.Id = cartId;
                await unitOfWork.CompleteAsync();
            }


            return ObjectMapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> DeleteCartAsync(long cartId)
        {
            var cart = await _cartRepository.FirstOrDefaultAsync(cartId);
            if (cart == null)
            {
                throw new UserFriendlyException("Ürün sepette bulunamadı!");
            }
            await _cartRepository.DeleteAsync(cartId);
            
            return ObjectMapper.Map<CartDto>(cart);
        }
    }
}
