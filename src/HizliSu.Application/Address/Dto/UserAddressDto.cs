using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HizliSu.Users.Dto;

namespace HizliSu.Address.Dto
{
    [AutoMapFrom(typeof(UserAddress))]
    public class UserAddressDto : FullAuditedEntityDto<long>
    {
        public string Title { get; set; }
        public long UserId { get; set; }
        public UserDto User { get; set; }
        public long CityId { get; set; }
        public CityDto City { get; set; }
        public long DistrictId { get; set; }
        public DistrictDto District { get; set; }
        public long NeighborhoodId { get; set; }
        public NeighborhoodDto Neighborhood { get; set; }
        public string StreetName { get; set; }
        public string No { get; set; }
        public string DoorNumber { get; set; }
        public string AddressDescription { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
