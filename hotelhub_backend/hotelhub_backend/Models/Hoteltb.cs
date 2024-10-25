using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Hoteltb
    {
        public Hoteltb()
        {
            Feedbacktbs = new HashSet<Feedbacktb>();
            Reservationtbs = new HashSet<Reservationtb>();
            Roomtbs = new HashSet<Roomtb>();
        }

        public int Id { get; set; }
        public string Hname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Image { get; set; }
        public string City { get; set; } = null!;
        public int IsApproved { get; set; }

        public virtual ICollection<Feedbacktb> Feedbacktbs { get; set; }
        public virtual ICollection<Reservationtb> Reservationtbs { get; set; }
        public virtual ICollection<Roomtb> Roomtbs { get; set; }
    }
}
