namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("conbudgetdet")]
    public partial class conbudgetdet
    {
        [Key]
        public int cdid { get; set; }
       
        [Column(TypeName = "numeric")]
        public decimal? vcid { get; set; }
         [StringLength(20)]
        public string pid { get; set; }
        public int? itemno { get; set; }

        [StringLength(20)]
        public string ctno { get; set; }

        [StringLength(150)]
        public string ctname { get; set; }

        [StringLength(20)]
        public string bdprodno { get; set; }

        [StringLength(100)]
        public string bdprodtitle { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pcount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pmoney { get; set; }

        [StringLength(20)]
        public string punit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? psummoney { get; set; }

        [StringLength(150)]
        public string pcomment { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ctpercent { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ctallmoney { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(1)]
        public string iforg { get; set; }

        public int? vtime { get; set; }

        [StringLength(20)]
        public string vctmodno { get; set; }

        [StringLength(20)]
        public string ctmodno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? wpcount { get; set; }

        [StringLength(20)]
        public string oldctno { get; set; }

        [StringLength(1)]
        public string ifuse { get; set; }

        [StringLength(1)]
        public string ifncontractuse { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? quorate { get; set; }

        [StringLength(2)]
        public string ifnow { get; set; }

        public int? cversion { get; set; }

        public int? clno { get; set; }

        public int? slyear { get; set; }

        [StringLength(20)]
        public string parent_prodno { get; set; }

        [StringLength(2)]
        public string modstatus { get; set; }

        public int? pleftmoney { get; set; }

        [StringLength(2)]
        public string bugcheck { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? acmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? varmoney { get; set; }

        [StringLength(10)]
        public string vendcomid { get; set; }

        public DateTime? psdate { get; set; }
        public DateTime? pedate { get; set; }

        [StringLength(1)]
        public string vendtype { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? npmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? npsummoney { get; set; }

        [StringLength(100)]
        public string workitem { get; set; }

        [StringLength(20)]
        public string wno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? moneyrate { get; set; }
    }
}
