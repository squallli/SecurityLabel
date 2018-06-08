namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bonusrate")]
    public partial class bonusrate
    {
        [Key]
        public int bid { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? vcid { get; set; }
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
    }
}
