using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace HizliSu.Address.Dto
{
    /// <summary>
    /// ilçe
    /// </summary>
    [AutoMapFrom(typeof(District))]
    public class DistrictDto : FullAuditedEntityDto<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long CityId { get; set; }
        public CityDto City { get; set; }
        public virtual List<NeighborhoodDto> Neighborhoods { get; set; }
    }
}
