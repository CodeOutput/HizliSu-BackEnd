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

namespace HizliSu.Address
{
    [AbpAuthorize()]
    public class CityAppService : ApplicationService, ICityAppService
    {
        private readonly IRepository<City, long> _cityRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CityAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<City, long> cityRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _cityRepository = cityRepository;
        }
  
        public async Task<ListResultDto<CityDto>> GetCityListAsync(bool? isActive)
        {
            List<City> cityList;
            if (isActive.HasValue)
            {
                cityList = (await _cityRepository.GetAllListAsync(x=>x.IsActive==isActive.Value)).OrderBy(x=>x.Name).ToList();
            }
            else
            {
                cityList = (await _cityRepository.GetAllListAsync()).OrderBy(x => x.Name).ToList();
            }

            return new ListResultDto<CityDto>(ObjectMapper.Map<List<CityDto>>(cityList));
        }

        public async Task<CityDto> GetCityAsync(long id)
        {
            var city = await _cityRepository.FirstOrDefaultAsync(id);
            return ObjectMapper.Map<CityDto>(city);
        }


        public async Task<CityDto> SaveCityAsync(CityRequest request)
        {
 
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("Şehir ismi giriniz!");
            }

            var city = await _cityRepository.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (city == null)
            {
                city = new City()
                {
                    Name = request.Name,
                    IsActive = request.IsActive
                };
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    var cityId = await _cityRepository.InsertAndGetIdAsync(city);
                    city.Id = cityId;
                    await unitOfWork.CompleteAsync();
                }

                return ObjectMapper.Map<CityDto>(city); 
            }

            throw new UserFriendlyException("Şehir zaten girilmiştir!");
        }

        public async Task<CityDto> UpdateCityAsync(long cityId, CityRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("Şehir ismi giriniz!");
            }

            var city = await _cityRepository.FirstOrDefaultAsync(cityId);
            if (city == null)
            {

                throw new UserFriendlyException("Şehir bulunamadı!"); 
            }

            city.Name = request.Name;
            city.IsActive = request.IsActive;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await _cityRepository.UpdateAsync(city);
                await unitOfWork.CompleteAsync();
            }

            return ObjectMapper.Map<CityDto>(city);
        }

        public async Task<CityDto> DeleteCityAsync(long cityId)
        {
            var city = await _cityRepository.FirstOrDefaultAsync(cityId);
            if (city == null)
            {
                throw new UserFriendlyException("Şehir bulunamadı!");
            }
            await _cityRepository.DeleteAsync(cityId);
            return ObjectMapper.Map<CityDto>(city);
        }
    }
}
