using Microsoft.AspNetCore.Http;

namespace HizliSu.Catalog.Request
{
    public class ManufacturerRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Content { get; set; }
        public int SortOrder { get; set; }
    }
}
