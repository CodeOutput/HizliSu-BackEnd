using Abp.Domain.Entities.Auditing;

namespace HizliSu.Address
{
    /// <summary>
    /// Mahalle
    /// </summary>
    public class Neighborhood : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long DistrictId { get; set; }
        public virtual District District { get; set; }
    }
}
