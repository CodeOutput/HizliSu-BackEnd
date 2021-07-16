using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;

namespace HizliSu.Address
{
    public interface INeighborhoodAppService : IApplicationService
    {
        Task<ListResultDto<NeighborhoodDto>> GetNeighborhoodListAsync(long districtId, bool? isActive);
        Task<NeighborhoodDto> SaveNeighborhoodAsync(NeighborhoodRequest request);
        Task<NeighborhoodDto> UpdateNeighborhoodAsync(long neighborhoodId, NeighborhoodRequest request);
        Task<NeighborhoodDto> DeleteNeighborhoodAsync(long neighborhoodId);
    }
}
