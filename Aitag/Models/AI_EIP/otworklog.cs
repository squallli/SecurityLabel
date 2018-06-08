namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("otworklog")]
    public partial class otworklog
    {
        [Key]
        public int otlogid { get; set; }

        [StringLength(1)]
        public string otype { get; set; }

        [StringLength(10)]
        public string osno { get; set; }

        [StringLength(1)]
        public string otstatus { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(50)]
        public string empname { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        [StringLength(10)]
        public string otid { get; set; }

        public DateTime? otlogsdate { get; set; }

        [StringLength(4)]
        public string otlogstime { get; set; }

        public DateTime? otlogedate { get; set; }

        [StringLength(4)]
        public string otlogetime { get; set; }

        public float? otloghour { get; set; }

        public float? otresthour { get; set; }

        public float? otmoneyhour { get; set; }

        [StringLength(500)]
        public string otcomment { get; set; }

        [StringLength(20)]
        public string arolestampid { get; set; }

        [StringLength(20)]
        public string rolestampid { get; set; }

        [StringLength(200)]
        public string rolestampidall { get; set; }

        [StringLength(200)]
        public string empstampidall { get; set; }

        [StringLength(500)]
        public string otback { get; set; }

        [StringLength(2)]
        public string ifotdell { get; set; }

        public DateTime? odate { get; set; }

        [StringLength(2)]
        public string ottype { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(500)]
        public string billtime { get; set; }

        public int? billflowid { get; set; }

        [StringLength(200)]
        public string empmeetsign { get; set; }

        [StringLength(2)]
        public string inout { get; set; }
    }
}
