using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HizliSu.General;

namespace HizliSu.Catalog.Dto
{

    [AutoMapFrom(typeof(Category))]
    public class CategoryDto : FullAuditedEntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FileDto Image { get; set; }
        public long ImageId { get; set; }
        public int SortOrder { get; set; }
    }
}
