using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Historytb
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

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

        public string HotelName { get; set; }

        public int Rent { get; set; }

        public string UserName { get; set; }
    }
}
