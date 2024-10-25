using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class RoomImagetb
    {
        public int Id { get; set; }
        public int Roomid { get; set; }
        public string Image { get; set; } = null!;

        public virtual Roomtb? Room { get; set; } = null!;
    }
}
