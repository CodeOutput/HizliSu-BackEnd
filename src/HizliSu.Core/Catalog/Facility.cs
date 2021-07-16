using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;

namespace HizliSu.Catalog
{
    public class Facility : FullAuditedEntity<long>
    {
        public long ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public virtual List<FacilityAttribute> FacilityAttributes { get; set; }
    }
}
