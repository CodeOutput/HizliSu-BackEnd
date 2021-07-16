using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HizliSu.General;

namespace HizliSu.Catalog.Dto
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : FullAuditedEntityDto<long>
    {
        public long CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public long ManufacturerId { get; set; }
        public ManufacturerDto Manufacturer { get; set; }
        public long? FacilityId { get; set; }
        public FacilityDto Facility { get; set; }
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
        public FileDto Image { get; set; }
    }
}
