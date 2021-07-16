using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace HizliSu.Address.Dto
{
    /// <summary>
    /// Mahalle
    /// </summary>
    [AutoMapFrom(typeof(Neighborhood))]
    public class NeighborhoodDto : FullAuditedEntityDto<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long DistrictId { get; set; }
        public DistrictDto District { get; set; }
    }
}
