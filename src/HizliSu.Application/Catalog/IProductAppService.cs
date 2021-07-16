using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Catalog.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HizliSu.Catalog.Request;

namespace HizliSu.Catalog
{
    public interface IProductAppService: IApplicationService
    {
        Task<ListResultDto<ProductDto>> GetProductListAsync();
        Task<ProductDto> GetProductDetailAsync(long id);
        Task<ProductDto> SaveProductAsync([FromForm] ProductRequest request);
        Task<ProductDto> UpdateProductAsync(long categoryId, [FromForm] ProductRequest request);
        Task<ProductDto> DeleteProductAsync(long categoryId);
    }
}
