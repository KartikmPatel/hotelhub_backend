using System;
using System.Collections.Generic;

namespace hotelhub_backend.Models
{
    public partial class RoomFeaturetb
    {
        public int Id { get; set; }
        public int FeatureId { get; set; }
        public int RoomId { get; set; }

        public virtual Featurestb Feature { get; set; } = null!;
        public virtual Roomtb Room { get; set; } = null!;
    }
}
