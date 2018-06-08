namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("workcard")]
    public partial class workcard
    {
        [Key]
        [StringLength(20)]
        public string wno { get; set; }

        [StringLength(2)]
        public string wstatus { get; set; }

        [StringLength(20)]
        public string custno { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(100)]
        public string wktitle { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(20)]
        public string prodno { get; set; }

        [StringLength(20)]
        public string ownman { get; set; }

        public DateTime? adddate { get; set; }

        public int? wkbudget { get; set; }

        [StringLength(20)]
        public string pwno { get; set; }

        [StringLength(2)]
        public string ifwh { get; set; }

        [StringLength(20)]
        public string whno { get; set; }

        public DateTime? prclosedate { get; set; }

        public int? putoffday { get; set; }

        public DateTime? closedate { get; set; }

        [StringLength(20)]
        public string closeman { get; set; }

        public int? slyear { get; set; }

        public int? slmonth { get; set; }

        public int? tkyear { get; set; }

        public int? tkmonth { get; set; }

        public int? hourcost { get; set; }

        public int? medicost { get; set; }

        public int? medicost_ac { get; set; }

        public int? princome { get; set; }

        public int? income { get; set; }

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

        [StringLength(500)]
        public string chknote { get; set; }

        public int? billflowid { get; set; }

        [StringLength(100)]
        public string modbackrolestampidall { get; set; }

        [StringLength(100)]
        public string modbackempstampidall { get; set; }

        [StringLength(500)]
        public string modback { get; set; }

        [StringLength(500)]
        public string modbackbilltime { get; set; }
    }
}
