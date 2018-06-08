namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("delholidaylog")]
    public partial class delholidaylog
    {
        [Key]
        public int hdellogid { get; set; }

        [StringLength(1)]
        public string hdellogstatus { get; set; }

        [StringLength(20)]
        public string hdno { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(50)]
        public string empname { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        [StringLength(20)]
        public string hsno { get; set; }

        [StringLength(10)]
        public string hdayid { get; set; }

        public DateTime? hlogsdate { get; set; }

        [StringLength(4)]
        public string hlogstime { get; set; }

        public DateTime? hlogedate { get; set; }

        [StringLength(4)]
        public string hlogetime { get; set; }

        public float? hloghour { get; set; }

        [StringLength(500)]
        public string hlogcomment { get; set; }

        [StringLength(20)]
        public string arolestampid { get; set; }

        [StringLength(20)]
        public string rolestampid { get; set; }

        [StringLength(200)]
        public string rolestampidall { get; set; }

        [StringLength(200)]
        public string empstampidall { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(500)]
        public string delback { get; set; }

        public DateTime? deldate { get; set; }

        public int? billflowid { get; set; }

        [StringLength(500)]
        public string billtime { get; set; }
    }
}
