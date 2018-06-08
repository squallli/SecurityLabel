namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("battalog")]
    public partial class battalog
    {
        [Key]
        public int blogid { get; set; }

        [StringLength(2)]
        public string keytype { get; set; }

        [StringLength(2)]
        public string blogtype { get; set; }

        [StringLength(1)]
        public string blogstatus { get; set; }

        [StringLength(15)]
        public string bsno { get; set; }

        [StringLength(15)]
        public string pbsno { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(50)]
        public string empname { get; set; }

        [StringLength(20)]
        public string addempid { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        public DateTime? blogsdate { get; set; }

        public DateTime? blogedate { get; set; }

        [StringLength(4)]
        public string blogstime { get; set; }

        [StringLength(4)]
        public string blogetime { get; set; }

        public int? bloghour { get; set; }

        public int? resthour { get; set; }

        [StringLength(10)]
        public string blogaddr { get; set; }

        [StringLength(100)]
        public string blogplace { get; set; }

        [StringLength(500)]
        public string blogcomment { get; set; }

        public int? bbillcount { get; set; }

        [StringLength(50)]
        public string replaceempid { get; set; }

        [StringLength(20)]
        public string arolestampid { get; set; }

        [StringLength(20)]
        public string rolestampid { get; set; }

        [StringLength(200)]
        public string rolestampidall { get; set; }

        [StringLength(200)]
        public string empstampidall { get; set; }

        [StringLength(500)]
        public string bback { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(2)]
        public string iftraholiday { get; set; }

        [StringLength(2)]
        public string ifhdell { get; set; }

        public DateTime? bdate { get; set; }

        public DateTime? bmoneydate { get; set; }

        [StringLength(50)]
        public string dptbossid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? exrate { get; set; }

        [StringLength(1000)]
        public string battamod { get; set; }

        [StringLength(500)]
        public string billtime { get; set; }

        [StringLength(20)]
        public string bfile { get; set; }

        public int? billflowid { get; set; }

        [StringLength(200)]
        public string empmeetsign { get; set; }

     //   [StringLength(20)]
     //   public string tmprolestampid { get; set; }
    }
}
