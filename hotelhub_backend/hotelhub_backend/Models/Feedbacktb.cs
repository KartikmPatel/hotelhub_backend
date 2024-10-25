using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Feedbacktb
    {
        public int Id { get; set; }
        public string? Comments { get; set; }
        public int? Rating { get; set; }
        public int Hid { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public int? ReadStatus { get; set; }

        public virtual Hoteltb? HidNavigation { get; set; } = null!;
        public virtual Roomtb? Room { get; set; } = null!;
        public virtual Usertb? User { get; set; } = null!;
    }
}
