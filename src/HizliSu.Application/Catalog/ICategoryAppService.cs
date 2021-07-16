using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Catalog.Dto;
using HizliSu.Catalog.Request;
using Microsoft.AspNetCore.Mvc;

namespace HizliSu.Catalog
{
    public interface ICategoryAppService: IApplicationService
    {
        Task<ListResultDto<CategoryDto>> GetCategoryListAsync();
        Task<CategoryDto> GetCategoryDetailAsync(long id);
        Task<CategoryDto> SaveCategoryAsync([FromForm] CategoryRequest request);
        Task<CategoryDto> UpdateCategoryAsync(long categoryId, [FromForm] CategoryRequest request);
        Task<CategoryDto> DeleteCategoryAsync(long categoryId);

    }
}
