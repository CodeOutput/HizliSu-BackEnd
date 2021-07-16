using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;

namespace HizliSu.Address
{
    public interface IDistrictAppService : IApplicationService
    {
        Task<ListResultDto<DistrictDto>> GetDistrictListAsync(long cityId, bool? isActive);
        Task<DistrictDto> SaveDistrictAsync(DistrictRequest request);
        Task<DistrictDto> UpdateDistrictAsync(long cityId, DistrictRequest request);
        Task<DistrictDto> DeleteDistrictAsync(long cartId);
    }
}
