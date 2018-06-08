namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("yearholidaydet")]
    public partial class yearholidaydet
    {
        [Key]
        public int hdid { get; set; }

        public int? yhsyear { get; set; }

        public int? yheyear { get; set; }

        public int? yhours { get; set; }

        [StringLength(10)]
        public string yhid { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
