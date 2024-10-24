using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class CancelBookingtb
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int Revid { get; set; }

        public virtual Reservationtb Rev { get; set; } = null!;
        public virtual Usertb UidNavigation { get; set; } = null!;
    }
}
