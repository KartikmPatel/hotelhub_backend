using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class FestivalDiscount
    {
        public FestivalDiscount()
        {
            Roomtbs = new HashSet<Roomtb>();
        }

        public int Id { get; set; }
        public string Festname { get; set; } = null!;
        public int Discount { get; set; }

        private DateTime _fesdate;
        public DateTime Fesdate
        {
            get => _fesdate.Date; // Ensure only the Date part is returned
            set => _fesdate = value.Date; // Store only the Date part
        }

        public virtual ICollection<Roomtb> Roomtbs { get; set; }
    }
}
