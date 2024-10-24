using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Historytb
    {
        public int Id { get; set; }
        public string HotelName { get; set; } = null!;
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public string CategoryName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int? Rent { get; set; }
    }
}
