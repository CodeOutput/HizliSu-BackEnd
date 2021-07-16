using System;
using Abp.Domain.Entities.Auditing;
using HizliSu.General;

namespace HizliSu.Catalog
{
    public class Category : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual File Image { get; set; }
        public long ImageId { get; set; }
        public int SortOrder { get; set; }
    }
}
