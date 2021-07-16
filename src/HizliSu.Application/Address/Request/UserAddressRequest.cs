namespace HizliSu.Address.Request
{
   public class UserAddressRequest
    {
        public string Title { get; set; }
        //public long UserId { get; set; }
        public long CityId { get; set; }
        public long DistrictId { get; set; }
        public long NeighborhoodId { get; set; }
        public string StreetName { get; set; }
        public string No { get; set; }
        public string DoorNumber { get; set; }
        public string AddressDescription { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
