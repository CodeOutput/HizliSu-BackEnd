using Abp.Domain.Entities.Auditing;

namespace HizliSu.Address
{
    /// <summary>
    /// Şehir, il
    /// </summary>
    public class City : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

    }
}
