using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;

namespace HizliSu.Address
{
    /// <summary>
    /// ilçe
    /// </summary>
    public class District : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public virtual IEnumerable<Neighborhood> Neighborhoods { get; set; }
    }
}
