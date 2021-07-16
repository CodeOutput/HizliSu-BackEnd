using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using HizliSu.Catalog;

namespace HizliSu.Address.Dto
{
    /// <summary>
    /// Şehir, il
    /// </summary>
    [AutoMapFrom(typeof(City))]
    public class CityDto : FullAuditedEntityDto<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

    }
}
