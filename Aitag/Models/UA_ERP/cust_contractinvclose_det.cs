namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class cust_contractinvclose_det
    {
        [Key]
        public int vctid { get; set; }

        public int? itemno { get; set; }
        [StringLength(20)]
        public string vcinvid { get; set; }

        [StringLength(20)]
        public string vcno { get; set; }

        public int? vserno { get; set; }
        [StringLength(20)]
        public string allcomid { get; set; }
        public string wcardno { get; set; }
        [StringLength(20)]
        public string dptid { get; set; }

        [StringLength(20)]
        public string bdprodno { get; set; }

        [StringLength(100)]
        public string bdprodtitle { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pcount { get; set; }

        [StringLength(20)]
        public string punit { get; set; }

        [StringLength(20)]
        public string subjectcode { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? psummoney { get; set; }

        [StringLength(100)]
        public string pcomment { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        public DateTime? dealdate { get; set; }
        [StringLength(50)]
        public string fromplace { get; set; }
        [StringLength(50)]
        public string toplace { get; set; }
        [StringLength(10)]
        public string planemoney { get; set; }
        [StringLength(10)]
        public string carmoney { get; set; }
        [StringLength(10)]
        public string othercarmoney { get; set; }
        [StringLength(10)]
        public string livemoney { get; set; }
        [StringLength(10)]
        public string eatmoney { get; set; }
        [StringLength(10)]
        public string othermoney { get; set; }
        [StringLength(30)]
        public string otherbill { get; set; }
        [StringLength(30)]
        public string pobill { get; set; }
        [StringLength(10)]
        public string itemcode { get; set; }

        [StringLength(10)]
        public string vendcomid { get; set; }

        public DateTime? pdsdate { get; set; }
        public DateTime? pdedate { get; set; }

        public decimal? remoney { get; set; }
        public decimal? remoneypki { get; set; }

        [StringLength(2)]
        public string ifagain { get; set; }

        [StringLength(10)]
        public string closetype { get; set; }


        [StringLength(10)]
        public string mdsummary { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? dismoney { get; set; }
    }
}
