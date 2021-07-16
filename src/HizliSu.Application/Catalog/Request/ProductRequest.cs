using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HizliSu.Catalog.Request
{
    public class ProductRequest
    {
        public long CategoryId { get; set; }
        public long ManufacturerId { get; set; }
        public long? FacilityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SellerCode { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }

        /// <summary>
        /// Birim adeti
        /// </summary>
        public int UnitQuantity { get; set; }

        ///// <summary>
        ///// Bir birimin fiyatı
        ///// </summary>
        //public decimal UnitPrice { get; set; }

        /// <summary>
        /// UnitQuantity*UnitPrice değerine eşittir.
        /// </summary>

        public decimal Price { get; set; }

        public bool Published { get; set; }

        public IFormFile Content { get; set; }
    }
}
