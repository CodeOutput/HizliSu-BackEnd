using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;

namespace HizliSu.Address
{
    public interface ICityAppService : IApplicationService
    {
        Task<ListResultDto<CityDto>> GetCityListAsync(bool? isActive);
        Task<CityDto> GetCityAsync(long id);
        Task<CityDto> SaveCityAsync(CityRequest request);
        Task<CityDto> UpdateCityAsync(long cityId, CityRequest request);
        Task<CityDto> DeleteCityAsync(long cartId);
    }
}
