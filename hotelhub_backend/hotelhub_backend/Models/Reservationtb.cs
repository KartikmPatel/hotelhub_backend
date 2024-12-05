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
        private DateTime _checkIn;
        public DateTime CheckIn
        {
            get => _checkIn.Date; // Ensure only the Date part is returned
            set => _checkIn = value.Date; // Store only the Date part
        }

        private DateTime _checkOut;
        public DateTime CheckOut
        {
            get => _checkOut.Date; // Ensure only the Date part is returned
            set => _checkOut = value.Date; // Store only the Date part
        }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public int Rent { get; set; }

        public int BookingStatus { get; set; }

        public virtual Hoteltb? HidNavigation { get; set; } = null!;
        public virtual Roomtb? Room { get; set; } = null!;
        public virtual Usertb? User { get; set; } = null!;
        public virtual ICollection<CancelBookingtb> CancelBookingtbs { get; set; }
    }
}
