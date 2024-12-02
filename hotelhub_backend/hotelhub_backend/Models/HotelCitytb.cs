using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class HotelCitytb
    {
        public int Id { get; set; }
        public int Hid { get; set; }
        public string City { get; set; } = null!;

        public virtual Hoteltb HidNavigation { get; set; } = null!;
    }
}
