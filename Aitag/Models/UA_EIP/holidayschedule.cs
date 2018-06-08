namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("holidayschedule")]
    public partial class holidayschedule
    {
        [Key]
        public int wsid { get; set; }

        [StringLength(50)]
        public string wstitle { get; set; }

        public DateTime? wsdate { get; set; }

        [StringLength(2)]
        public string wstype { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(10)]
        public string yhid { get; set; }
    }
}
