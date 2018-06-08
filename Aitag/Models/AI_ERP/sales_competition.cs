namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sales_competition
    {
        [Key]
        [Column(TypeName = "numeric")]
        public decimal ccid { get; set; }

        [StringLength(2)]
        public string newtype { get; set; }

        [StringLength(200)]
        public string custtitle { get; set; }

        [StringLength(20)]
        public string dptid { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(200)]
        public string prodtitle { get; set; }

        [StringLength(200)]
        public string corpitem { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? salespmoney { get; set; }

        [StringLength(2)]
        public string steptype { get; set; }

        [StringLength(100)]
        public string exetime { get; set; }

        [StringLength(1000)]
        public string salescomment { get; set; }

        [StringLength(1000)]
        public string lawcomment { get; set; }

        [StringLength(1000)]
        public string fincomment { get; set; }

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

        public DateTime? adddate { get; set; }

        [StringLength(2)]
        public string slogtype { get; set; }

        [StringLength(1)]
        public string slogstatus { get; set; }

        [StringLength(10)]
        public string custlevel1 { get; set; }
        [StringLength(10)]
        public string custlevel2 { get; set; }
        [StringLength(10)]
        public string custlevel3 { get; set; }
        [StringLength(2)]
        public string iflaw { get; set; }
        [StringLength(2)]
        public string iffin { get; set; }
        [StringLength(20)]
        public string indclass { get; set; }
        [StringLength(2)]
        public string ifget { get; set; }

        [StringLength(30)]
        public string getcomtitle { get; set; }
    }
}
