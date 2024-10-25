using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Roomtb
    {
        public Roomtb()
        {
            Feedbacktbs = new HashSet<Feedbacktb>();
            Reservationtbs = new HashSet<Reservationtb>();
            RoomFacilitytbs = new HashSet<RoomFacilitytb>();
            RoomFeaturetbs = new HashSet<RoomFeaturetb>();
            RoomImagetbs = new HashSet<RoomImagetb>();
        }

        public int Id { get; set; }
        public int Roomcategoryid { get; set; }
        public int AdultCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public int Quantity { get; set; }
        public int ActiveStatus { get; set; }
        public int Hid { get; set; }
        public int Rent { get; set; }
        public int? Discount { get; set; }
        public int? FestivalId { get; set; }

        public virtual FestivalDiscount? Festival { get; set; }
        public virtual RoomCategorytb? Roomcategory { get; set; }
        public virtual Hoteltb? HidNavigation { get; set; }
        public virtual ICollection<Feedbacktb> Feedbacktbs { get; set; }
        public virtual ICollection<Reservationtb> Reservationtbs { get; set; }
        public virtual ICollection<RoomFacilitytb> RoomFacilitytbs { get; set; }
        public virtual ICollection<RoomFeaturetb> RoomFeaturetbs { get; set; }
        public virtual ICollection<RoomImagetb> RoomImagetbs { get; set; }
    }
}
