namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("purchase")]
    public partial class purchase
    {
        [Key]
        [StringLength(20)]
        public string pid { get; set; }

        [StringLength(2)]
        public string vendtype { get; set; }

        [StringLength(20)]
        public string pdno { get; set; }

        [StringLength(1)]
        public string pstatus { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        public DateTime? pdate { get; set; }

        [StringLength(2)]
        public string taxtype { get; set; }

        [StringLength(20)]
        public string ownman { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ptaxmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pallmoney { get; set; }

        [StringLength(20)]
        public string allcomid { get; set; }

        [StringLength(1000)]
        public string pcomment { get; set; }

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

        public int? billflowid { get; set; }

        [StringLength(100)]
        public string modbackrolestampidall { get; set; }

        [StringLength(100)]
        public string modbackempstampidall { get; set; }

        [StringLength(500)]
        public string modback { get; set; }

        [StringLength(500)]
        public string modbackbilltime { get; set; }


        [StringLength(200)]
        public string campainetitle { get; set; }

        public DateTime? psdate { get; set; }
        public DateTime? pedate { get; set; }
        [StringLength(20)]
        public string prodid { get; set; }

       
    }
}
