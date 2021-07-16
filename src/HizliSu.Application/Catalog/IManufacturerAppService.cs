using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Catalog.Dto;
using HizliSu.Catalog.Request;
using Microsoft.AspNetCore.Mvc;

namespace HizliSu.Catalog
{
    public interface IManufacturerAppService : IApplicationService
    {
        Task<ListResultDto<ManufacturerDto>> GetManufacturerListAsync();
        Task<ManufacturerDto> GetManufacturerDetailAsync(long id);
        Task<ManufacturerDto> SaveManufacturerAsync([FromForm] ManufacturerRequest request);
        Task<ManufacturerDto> UpdateManufacturerAsync(long manufacturerId, [FromForm] ManufacturerRequest request);
        Task<ManufacturerDto> DeleteManufacturerAsync(long manufacturerId);


        Task<ListResultDto<FacilityDto>> GetFacilityListAsync(long manufacturerId);
        Task<FacilityDto> SaveFacilityAsync(FacilityRequest request);
        Task<FacilityDto> UpdateFacilityAsync(long facilityId, FacilityRequest request);
        Task<FacilityDto> DeleteFacilityAsync(long facilityId);


        Task<FacilityAttributeDto> SaveFacilityAttributeAsync(FacilityAttributeRequest request);
        Task<FacilityAttributeDto> UpdateFacilityAttributeAsync(long facilityAttributeId, FacilityAttributeRequest request);
        Task<FacilityAttributeDto> DeleteFacilityAttributeAsync(long facilityAttributeId);


    }
}
