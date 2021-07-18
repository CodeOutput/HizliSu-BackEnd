using System;

namespace HizliSu.Orders.Dto
{
   public class OrderPagingCo
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
  
        public long? OrderStatusId { get; set; }
        public DateTime? CreationTime { get; set; }
  
        public string OrderNote { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DistrictName { get; set; }
        public string NeighborhoodName { get; set; }

 



        public int ResultCount { get; set; }
        public int PageNo { get; set; }
    }
}
