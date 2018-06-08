namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("holidaylog")]
    public partial class holidaylog
    {
        [Key]
        public int hlogid { get; set; }

        [StringLength(2)]
        public string hlogtype { get; set; }

        [StringLength(15)]
        public string hsno { get; set; }

        [StringLength(15)]
        public string phsno { get; set; }

        [StringLength(1)]
        public string hlogstatus { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(50)]
        public string empname { get; set; }

        [StringLength(20)]
        public string addempid { get; set; }

        [StringLength(100)]
        public string dptid { get; set; }

        [StringLength(10)]
        public string hdayid { get; set; }

        public DateTime? factsday { get; set; }

        public DateTime? hlogsdate { get; set; }

        [StringLength(4)]
        public string hlogstime { get; set; }

        public DateTime? hlogedate { get; set; }

        [StringLength(4)]
        public string hlogetime { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? hloghour { get; set; }

        [StringLength(2)]
        public string hlogaddr { get; set; }

        [StringLength(500)]
        public string hlogcomment { get; set; }

        [StringLength(2)]
        public string ifusecard { get; set; }

        [StringLength(20)]
        public string replaceempid { get; set; }

        [StringLength(200)]
        public string roledptid { get; set; }

        [StringLength(20)]
        public string arolestampid { get; set; }

        [StringLength(20)]
        public string rolestampid { get; set; }

        [StringLength(200)]
        public string rolestampidall { get; set; }

        [StringLength(200)]
        public string empstampidall { get; set; }

        [StringLength(2)]
        public string iftransfered { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(500)]
        public string hback1 { get; set; }

        public DateTime? hdate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? hdelloghour { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? hlasthour { get; set; }

        public DateTime? hjumpdate { get; set; }

        [StringLength(50)]
        public string dptbossid { get; set; }

        [StringLength(50)]
        public string hfile { get; set; }

        [StringLength(1000)]
        public string holidaymod { get; set; }

        [StringLength(20)]
        public string reempid { get; set; }

        [StringLength(50)]
        public string reempname { get; set; }

        [StringLength(500)]
        public string billtime { get; set; }

        public int? billflowid { get; set; }

        [StringLength(200)]
        public string empmeetsign { get; set; }

        [StringLength(20)]
        public string tmpempno { get; set; }

        [StringLength(20)]
        public string replaceempidsigned { get; set; }

        public DateTime? replaceempiddate { get; set; }

        [StringLength(20)]
        public string tmprolestampid { get; set; }
    }
}
