namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("allcompany_rate")]
    public partial class allcompany_rate
    {
        [Key]
        public int bid { get; set; }
        [StringLength(10)]
        public string allcomid { get; set; }

        [StringLength(20)]
        public string mcno { get; set; }

        [StringLength(500)]
        public string mdno { get; set; }


        [StringLength(500)]
        public string nonmdno { get; set; }

       
        [StringLength(2)]
        public string bseason { get; set; }

        [StringLength(2)]
        public string btype { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? bsmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? bemoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? brate { get; set; }

        [StringLength(100)]
        public string bcomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(2)]
        public string ifzero { get; set; }
        [StringLength(2)]
        public string bfday_discount { get; set; }

        [StringLength(200)]
        public string nonclosetype { get; set; }

        [StringLength(2)]
        public string iftax { get; set; }
    }
}
