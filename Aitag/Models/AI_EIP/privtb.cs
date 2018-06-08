namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("privtb")]
    public partial class privtb
    {
        [Key]
        public int pid { get; set; }

        public int? sid { get; set; }

        public int? psid { get; set; }

        public int? uplink { get; set; }

        [StringLength(20)]
        public string bid { get; set; }

        [StringLength(1)]
        public string chk { get; set; }

        [StringLength(1)]
        public string subread { get; set; }

        [StringLength(1)]
        public string subadd { get; set; }

        [StringLength(1)]
        public string submod { get; set; }

        [StringLength(1)]
        public string subdel { get; set; }

        public int? subcount { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
