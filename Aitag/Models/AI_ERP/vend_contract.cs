namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vend_contract
    {
        [Key]
        [Column(TypeName = "numeric")]
        public decimal vcid { get; set; }

        [StringLength(20)]
        public string vcno { get; set; }

        [StringLength(1)]
        public string vcstatus { get; set; }

        [StringLength(2)]
        public string vctype { get; set; }

        [StringLength(2)]
        public string vendtype { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        public DateTime? vcdate { get; set; }

        public DateTime? vcsdate { get; set; }

        public DateTime? vcedate { get; set; }

        [StringLength(2)]
        public string taxtype { get; set; }

        [StringLength(20)]
        public string ownman { get; set; }

        [StringLength(20)]
        public string pdno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vcmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vctaxmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vcallmoney { get; set; }

        [StringLength(20)]
        public string allcomid { get; set; }

        [StringLength(1000)]
        public string vccomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(4)]
        public string billno { get; set; }

        [StringLength(10)]
        public string arolestampid { get; set; }

        [StringLength(10)]
        public string rolestampid { get; set; }

        [StringLength(100)]
        public string rolestampidall { get; set; }

        [StringLength(100)]
        public string empstampidall { get; set; }

        [StringLength(50)]
        public string backrolestampid { get; set; }

        [StringLength(100)]
        public string backrolestampidall { get; set; }

        [StringLength(100)]
        public string backempstampidall { get; set; }

        [StringLength(500)]
        public string rback { get; set; }

        [StringLength(500)]
        public string billtime { get; set; }

        [StringLength(500)]
        public string backbilltime { get; set; }

        [StringLength(100)]
        public string modbackrolestampidall { get; set; }

        [StringLength(100)]
        public string modbackempstampidall { get; set; }

        [StringLength(500)]
        public string modback { get; set; }

        [StringLength(500)]
        public string modbackbilltime { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? prodid { get; set; }
    }
}
