namespace HizliSu.Catalog.Request
{
   public class FacilityAttributeRequest
    {
        public long FacilityId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
