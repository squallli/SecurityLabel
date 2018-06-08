namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("workhour")]
    public partial class workhour
    {
        [Key]
        public int wid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal whid { get; set; }

        [StringLength(1)]
        public string wstatus { get; set; }

        [StringLength(5)]
        public string corp_no { get; set; }

        [StringLength(20)]
        public string whno { get; set; }

        [StringLength(20)]
        public string voucher_no { get; set; }

        public int? itemno { get; set; }

        public DateTime? wadddate { get; set; }

        [StringLength(20)]
        public string dptid { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(20)]
        public string empno { get; set; }

        public int? put_in_times { get; set; }

        [StringLength(5)]
        public string work_no_code { get; set; }

        [StringLength(20)]
        public string custno { get; set; }

        [StringLength(20)]
        public string whdptid { get; set; }

        [StringLength(1000)]
        public string whcomment { get; set; }

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
    }
}
