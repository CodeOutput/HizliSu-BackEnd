using Abp.Domain.Entities.Auditing;

namespace HizliSu.Catalog
{
    public class FacilityAttribute : FullAuditedEntity<long>
    {
        public long FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
