using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Catalog.Dto;
using HizliSu.Catalog.Request;
using HizliSu.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HizliSu.Catalog
{
    [AbpAuthorize()]
    public class ProductAppService : ApplicationService, IProductAppService
    {
        private readonly IRepository<Product, long> _productRepository;
        private readonly IRepository<File, long> _fileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProductAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<File, long> fileRepository, IRepository<Product, long> productRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _fileRepository = fileRepository;
            _productRepository = productRepository;
        }

        public async Task<ListResultDto<ProductDto>> GetProductListAsync()
        {
            List<Product> productList = await _productRepository
                .GetAllIncluding(c=>c.Category, i => i.Image, i => i.Manufacturer, i => i.Facility)
                .Where(x=>x.Published)
                .OrderBy(x=>x.Manufacturer.SortOrder).ThenBy(o=>o.Name).ToListAsync();
            return new ListResultDto<ProductDto>(ObjectMapper.Map<List<ProductDto>>(productList));
        }

        public async Task<ProductDto> GetProductDetailAsync(long id)
        {
            var product = await _productRepository.GetAllIncluding(i => i.Image).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                throw new UserFriendlyException("Ürün bulunamadı!");
            }
            return ObjectMapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> SaveProductAsync([FromForm] ProductRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");

            if (request.Content == null)
            {
                throw new UserFriendlyException("Dosya yüklemediniz!");
            }
            
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("Ürün adı giriniz!");
            }

            if (request.UnitQuantity < 1)
            {
                throw new UserFriendlyException("Paketteki Adet Sayısı 0 dan büyük olmak zorundadır!");
            }
            
            if (request.CategoryId < 1)
            {
                throw new UserFriendlyException("Lütfen kategori seçiniz!");
            }
            
            if (request.ManufacturerId < 1)
            {
                throw new UserFriendlyException("Lütfen üretici seçiniz!");
            }

           

            long length = request.Content?.Length ?? 0;

            await using var fileStream = request.Content.OpenReadStream();
            byte[] bytes = new byte[length];
            await fileStream.ReadAsync(bytes, 0, (int)request.Content.Length);




            var file = new File()
            {
                FileName = request.Content.FileName,
                UserFileName = request.Content.FileName,
                Length = request.Content.Length,
                ContentType = request.Content.ContentType.ToLower(new CultureInfo("en-US", false)),
                Content = bytes
            };

            var product = new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Barcode = request.Barcode,
                Price = request.Price,
                Published = request.Published,
                SellerCode = request.SellerCode,
                StockQuantity = request.StockQuantity,
                UnitQuantity = request.UnitQuantity,
                FacilityId = request.FacilityId,
                CategoryId = request.CategoryId,
                ManufacturerId = request.ManufacturerId
            };

            product.UnitPrice = request.Price / request.UnitQuantity;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var addImageId = await _fileRepository.InsertAndGetIdAsync(file);
                product.ImageId = addImageId;
                var productId = await _productRepository.InsertAndGetIdAsync(product);
                product.Id = productId;
                await unitOfWork.CompleteAsync();
            }
            return ObjectMapper.Map<ProductDto>(product);
        }





        public async Task<ProductDto> UpdateProductAsync(long productId, [FromForm] ProductRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");

            long length = request.Content?.Length ?? 0;

            var product = await _productRepository.FirstOrDefaultAsync(productId);

            if (product == null)
                throw new UserFriendlyException("Ürün bulunamadı!");

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("Ürün adı giriniz!");
            }

            if (request.UnitQuantity < 1)
            {
                throw new UserFriendlyException("Paketteki Adet Sayısı 0 dan büyük olmak zorundadır!");
            }

            if (request.CategoryId < 1)
            {
                throw new UserFriendlyException("Lütfen kategori seçiniz!");
            }

            if (request.ManufacturerId < 1)
            {
                throw new UserFriendlyException("Lütfen üretici seçiniz!");
            }

            var file = await _fileRepository.FirstOrDefaultAsync(product.ImageId);


            if (length > 0)
            {
                if (request.Content != null)
                {
                    await using var fileStream = request.Content.OpenReadStream();
                    byte[] bytes = new byte[length];
                    await fileStream.ReadAsync(bytes, 0, (int)request.Content.Length);

                    file.FileName = request.Content.FileName;
                    file.UserFileName = request.Content.FileName;
                    file.Length = request.Content.Length;
                    file.ContentType = request.Content.ContentType.ToLower(new CultureInfo("en-US", false));
                    file.Content = bytes;
                }
            }


            product.Name = request.Name;
            product.Description = request.Description;
            product.Name = request.Name;
            product.Description = request.Description;
            product.Barcode = request.Barcode;
            product.Price = request.Price;
            product.Published = request.Published;
            product.SellerCode = request.SellerCode;
            product.UnitPrice = request.Price / request.UnitQuantity;
            product.StockQuantity = request.StockQuantity;
            product.UnitQuantity = request.UnitQuantity;
            product.CategoryId = request.CategoryId;
            product.ManufacturerId = request.ManufacturerId;
            product.FacilityId = request.FacilityId;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                if (length > 0)
                {
                    await _fileRepository.UpdateAsync(file);
                }
                await _productRepository.UpdateAsync(product);
                await unitOfWork.CompleteAsync();
            }
            return ObjectMapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> DeleteProductAsync(long productId)
        {
            var product = await _productRepository.FirstOrDefaultAsync(productId);
            if (product == null)
            {
                throw new UserFriendlyException("Ürün bulunamadı!");
            }

            await _productRepository.DeleteAsync(productId);
            await _fileRepository.DeleteAsync(product.ImageId);

            return ObjectMapper.Map<ProductDto>(product);
        }
    }
}
