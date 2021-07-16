using Abp.Domain.Entities.Auditing;
using HizliSu.Authorization.Users;

namespace HizliSu.Address
{
    public class UserAddress : FullAuditedEntity<long>
    {  
        public string Title { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }

        public long DistrictId { get; set; }
        public virtual District District { get; set; }

        public long NeighborhoodId { get; set; }
        public virtual Neighborhood Neighborhood { get; set; }


        public string StreetName { get; set; }
        public string No { get; set; }
        public string DoorNumber { get; set; }
        public string AddressDescription { get; set; }
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

    }
}
