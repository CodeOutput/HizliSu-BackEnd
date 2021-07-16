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
    public class CategoryAppService : ApplicationService, ICategoryAppService
    {
        private readonly IRepository<Category, long> _categoryRepository;
        private readonly IRepository<File, long> _fileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CategoryAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<File, long> fileRepository, IRepository<Category, long> categoryRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _fileRepository = fileRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ListResultDto<CategoryDto>> GetCategoryListAsync()
        {
            List<Category> categoryList = await _categoryRepository.GetAllIncluding(i=>i.Image).OrderBy(x=>x.SortOrder).ToListAsync();
            return new ListResultDto<CategoryDto>(ObjectMapper.Map<List<CategoryDto>>(categoryList));
        }

        public async Task<CategoryDto> GetCategoryDetailAsync(long id)
        {
            var category = await _categoryRepository.GetAllIncluding(i=>i.Image).FirstOrDefaultAsync(x=>x.Id == id);
            if (category == null)
            {
                throw new UserFriendlyException("Kategori bulunamadı!");
            }
            return ObjectMapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> SaveCategoryAsync([FromForm] CategoryRequest request)
        {
            if(request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");

            if (request.Content == null)
            {
                throw new UserFriendlyException("Dosya yüklemediniz!");
            }

            long length = request.Content?.Length??0;

            await using var fileStream = request.Content.OpenReadStream();
            byte[] bytes = new byte[length];
            await fileStream.ReadAsync(bytes, 0, (int) request.Content.Length);


            var file = new File()
            {
                FileName = request.Content.FileName,
                UserFileName = request.Content.FileName,
                Length = request.Content.Length,
                ContentType = request.Content.ContentType.ToLower(new CultureInfo("en-US", false)),
                Content = bytes
            };

            var category = new Category()
            {
                Name = request.Name,
                Description = request.Description,
                SortOrder = request.SortOrder
            };
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var addImageId = await _fileRepository.InsertAndGetIdAsync(file);
                category.ImageId = addImageId;
                var categoryId = await _categoryRepository.InsertAndGetIdAsync(category);
                category.Id = categoryId;
                await unitOfWork.CompleteAsync();
            }
            return ObjectMapper.Map<CategoryDto>(category);
        }





        public async Task<CategoryDto> UpdateCategoryAsync(long categoryId, [FromForm] CategoryRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
  
            long length = request.Content?.Length??0;

            var category = await _categoryRepository.FirstOrDefaultAsync(categoryId);

            if (category  == null)
                throw new UserFriendlyException("Kategori bulunamadı!");
            

            var file = await _fileRepository.FirstOrDefaultAsync(category.ImageId);


            if (length > 0)
            {
                if (request.Content != null)
                {
                    await using var fileStream = request.Content.OpenReadStream();
                    byte[] bytes = new byte[length];
                    await fileStream.ReadAsync(bytes, 0, (int) request.Content.Length);
                    
                    file.FileName = request.Content.FileName;
                    file.UserFileName = request.Content.FileName;
                    file.Length = request.Content.Length;
                    file.ContentType = request.Content.ContentType.ToLower(new CultureInfo("en-US", false));
                    file.Content = bytes;
                }
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.SortOrder = request.SortOrder;
            
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                if (length > 0)
                {
                   await _fileRepository.UpdateAsync(file);
                }
                await _categoryRepository.UpdateAsync(category);
                await unitOfWork.CompleteAsync();
            }
            return ObjectMapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> DeleteCategoryAsync(long categoryId)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(categoryId);
            if (category == null)
            {
                throw new UserFriendlyException("Kategori bulunamadı!");
            }

            await _categoryRepository.DeleteAsync(categoryId);
            await _fileRepository.DeleteAsync(category.ImageId);

            return ObjectMapper.Map<CategoryDto>(category);
        }
    }
}
