namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cardjudgelog")]
    public partial class cardjudgelog
    {
        [Key]
        public int cjid { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(20)]
        public string empname { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        public DateTime? clogdate { get; set; }

        [StringLength(20)]
        public string clogstime { get; set; }

        [StringLength(20)]
        public string clogetime { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cloghour { get; set; }

        [StringLength(2)]
        public string clogstatus { get; set; }

        [StringLength(200)]
        public string clogcomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(2)]
        public string cchkstatus { get; set; }

        [StringLength(500)]
        public string cchkcomment { get; set; }

        [StringLength(20)]
        public string cchkownman { get; set; }

        public DateTime? cchkowndate { get; set; }
    }
}
