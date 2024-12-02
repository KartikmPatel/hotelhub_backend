using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Usertb
    {
        public Usertb()
        {
            CancelBookingtbs = new HashSet<CancelBookingtb>();
            Feedbacktbs = new HashSet<Feedbacktb>();
            Reservationtbs = new HashSet<Reservationtb>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Mno { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Gender { get; set; } = null!;

        public virtual ICollection<CancelBookingtb> CancelBookingtbs { get; set; }
        public virtual ICollection<Feedbacktb> Feedbacktbs { get; set; }
        public virtual ICollection<Reservationtb> Reservationtbs { get; set; }
    }
}
