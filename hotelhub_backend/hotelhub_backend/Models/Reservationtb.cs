using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Reservationtb
    {
        public Reservationtb()
        {
            CancelBookingtbs = new HashSet<CancelBookingtb>();
        }

        public int Id { get; set; }
        public int Hid { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public int Rent { get; set; }

        public virtual Hoteltb HidNavigation { get; set; } = null!;
        public virtual Roomtb Room { get; set; } = null!;
        public virtual Usertb User { get; set; } = null!;
        public virtual ICollection<CancelBookingtb> CancelBookingtbs { get; set; }
    }
}
