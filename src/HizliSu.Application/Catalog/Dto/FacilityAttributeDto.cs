using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace HizliSu.Catalog.Dto
{

    [AutoMapFrom(typeof(FacilityAttribute))]
    public class FacilityAttributeDto : FullAuditedEntityDto<long>
    {
        public long FacilityId { get; set; }
        public FacilityDto Facility { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
