using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class RoomFacilitytb
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public int RoomId { get; set; }

        public virtual Facilitytb? Facility { get; set; } = null!;
        public virtual Roomtb? Room { get; set; } = null!;
    }
}
