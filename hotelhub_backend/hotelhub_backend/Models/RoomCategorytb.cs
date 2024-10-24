using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class RoomCategorytb
    {
        public RoomCategorytb()
        {
            Roomtbs = new HashSet<Roomtb>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Roomtb> Roomtbs { get; set; }
    }
}
