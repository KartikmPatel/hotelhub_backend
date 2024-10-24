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
        public DateOnly Fesdate { get; set; }

        public virtual ICollection<Roomtb> Roomtbs { get; set; }
    }
}
