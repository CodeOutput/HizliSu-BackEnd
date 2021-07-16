using System;
using Abp.Domain.Entities.Auditing;

namespace HizliSu.Tax
{
    public class TaxRate : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }

    }
}
