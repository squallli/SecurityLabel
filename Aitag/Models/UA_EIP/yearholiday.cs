namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("yearholiday")]
    public partial class yearholiday
    {
        [Key]
        [StringLength(10)]
        public string yhid { get; set; }

        [StringLength(100)]
        public string yhtitle { get; set; }

        [StringLength(4)]
        public string yhstime { get; set; }

        [StringLength(4)]
        public string yhetime { get; set; }

        [StringLength(4)]
        public string ytstime { get; set; }

        [StringLength(4)]
        public string ytetime { get; set; }

        [StringLength(4)]
        public string ydstime { get; set; }

        [StringLength(4)]
        public string ydetime { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? yresthour { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? yhhour { get; set; }

        [StringLength(50)]
        public string clockpath { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
