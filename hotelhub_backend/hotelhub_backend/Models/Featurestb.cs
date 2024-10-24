using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class Featurestb
    {
        public Featurestb()
        {
            RoomFeaturetbs = new HashSet<RoomFeaturetb>();
        }

        public int Id { get; set; }
        public string FeatureName { get; set; } = null!;
        public string? Image { get; set; }

        public virtual ICollection<RoomFeaturetb> RoomFeaturetbs { get; set; }
    }
}
