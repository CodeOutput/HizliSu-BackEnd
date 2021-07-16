using Abp.Domain.Entities.Auditing;
using HizliSu.General;

namespace HizliSu.Catalog
{

    public class Product : FullAuditedEntity<long>
    {
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; } 
        public long ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }

        public long? FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SellerCode { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }

        /// <summary>
        /// Birim adeti
        /// </summary>
        public int UnitQuantity { get; set; }

        /// <summary>
        /// Bir birimin fiyatı
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// UnitQuantity*UnitPrice değerine eşittir.
        /// </summary>
        public decimal Price { get; set; }

        public bool Published { get; set; }


        public long ImageId { get; set; }
        public virtual File Image { get; set; }
        //public Guid TaxRateId { get; set; }
        //public virtual TaxRate TaxRate { get; set; }
        
    }
}
