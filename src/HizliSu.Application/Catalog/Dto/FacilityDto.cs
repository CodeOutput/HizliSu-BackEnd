using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace HizliSu.Catalog.Dto
{
    [AutoMapFrom(typeof(Facility))]
    public class FacilityDto : FullAuditedEntityDto<long>
    {
        public long ManufacturerId { get; set; }
        public ManufacturerDto Manufacturer { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public List<FacilityAttributeDto> FacilityAttributes { get; set; }
    }
}
