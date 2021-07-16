using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;
using Microsoft.EntityFrameworkCore;

namespace HizliSu.Address
{
    [AbpAuthorize()]
    public class DistrictAppService : ApplicationService, IDistrictAppService
    {
        private readonly IRepository<District, long> _districtRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DistrictAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<District, long> districtRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _districtRepository = districtRepository;
        }
  
        public async Task<ListResultDto<DistrictDto>> GetDistrictListAsync(long cityId, bool? isActive)
        {

            List<District> districtList;
            if (isActive.HasValue)
            {
                districtList = await _districtRepository.GetAllIncluding(x=>x.Neighborhoods).Where(x=>x.CityId == cityId && x.IsActive==isActive.Value).ToListAsync();
            }
            else
            {
                districtList = await _districtRepository.GetAllIncluding(x => x.Neighborhoods).Where(x => x.CityId == cityId).ToListAsync();
            }
            return new ListResultDto<DistrictDto>(ObjectMapper.Map<List<DistrictDto>>(districtList));
        }
           

        public async Task<DistrictDto> SaveDistrictAsync(DistrictRequest request)
        {
 
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("İlçe ismi giriniz!");
            }

            var district = await _districtRepository.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (district == null)
            {
                district = new District()
                {
                    Name = request.Name,
                    CityId = request.CityId,
                    IsActive = request.IsActive
                };
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    var districtId = await _districtRepository.InsertAndGetIdAsync(district);
                    district.Id = districtId;
                    await unitOfWork.CompleteAsync();
                }

                return ObjectMapper.Map<DistrictDto>(district); 
            }

            throw new UserFriendlyException("İlçe zaten girilmiştir!");
        }

        public async Task<DistrictDto> UpdateDistrictAsync(long districtId, DistrictRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("İlçe ismi giriniz!");
            }

            var district = await _districtRepository.FirstOrDefaultAsync(districtId);
            if (district == null)
            {

                throw new UserFriendlyException("Kayıt bulunamadı!"); 
            }

            district.Name = request.Name;
            district.IsActive = request.IsActive;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await _districtRepository.UpdateAsync(district);
                await unitOfWork.CompleteAsync();
            }

            return ObjectMapper.Map<DistrictDto>(district);
        }

        public async Task<DistrictDto> DeleteDistrictAsync(long districtId)
        {
            var district = await _districtRepository.FirstOrDefaultAsync(districtId);
            if (district == null)
            {
                throw new UserFriendlyException("Kayıt bulunamadı!");
            }
            await _districtRepository.DeleteAsync(districtId);
            return ObjectMapper.Map<DistrictDto>(district);
        }
    }
}
