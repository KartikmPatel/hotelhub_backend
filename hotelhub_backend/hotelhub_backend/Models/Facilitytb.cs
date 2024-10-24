using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Facilitytb
    {
        public Facilitytb()
        {
            RoomFacilitytbs = new HashSet<RoomFacilitytb>();
        }

        public int Id { get; set; }
        public string FacilityName { get; set; } = null!;
        public string? Image { get; set; }

        public virtual ICollection<RoomFacilitytb> RoomFacilitytbs { get; set; }
    }
}
