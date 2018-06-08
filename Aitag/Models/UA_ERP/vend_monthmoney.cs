namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vend_monthmoney
    {
        [Key]
        public int vsid { get; set; }

        [StringLength(2)]
        public string vtype { get; set; }

        [StringLength(2)]
        public string bseason { get; set; }

        public int? slyear { get; set; }

        public int? slmonth { get; set; }

        [StringLength(20)]
        public string vendcomid { get; set; }

        [StringLength(20)]
        public string custcomid { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(20)]
        public string pdno { get; set; }

        [StringLength(20)]
        public string prodno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pallmoney { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pallbonusmoney { get; set; }
    }
}
