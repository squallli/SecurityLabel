namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vend_contractinv
    {
        [Key]
        public int vcinvid { get; set; }

        [StringLength(1)]
        public string vstatus { get; set; }

        [StringLength(20)]
        public string vcno { get; set; }

        public int? vserno { get; set; }

        [StringLength(2)]
        public string vctype { get; set; }
        [StringLength(2)]
        public string vcsubtype { get; set; }

        [StringLength(2)]
        public string costtype { get; set; }

        [StringLength(2)]
        public string currency { get; set; }

        public DateTime? vadate { get; set; }

        public DateTime? vpdate { get; set; }
        
        public DateTime? spdate { get; set; }

        public DateTime? deaddate { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(20)]
        public string wcardno { get; set; }

        [StringLength(20)]
        public string vendno { get; set; }

        [StringLength(20)]
        public string ownman { get; set; }
        [StringLength(20)]
        public string owndptid { get; set; }

        [StringLength(20)]
        public string payman { get; set; }

        [StringLength(2)]
        public string paytype { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? totalmoney { get; set; }

        [Column(TypeName = "ntext")]
        public string vcomment { get; set; }

        [Column(TypeName = "ntext")]
        public string othercomment { get; set; }

        [StringLength(4)]
        public string billno { get; set; }

        [StringLength(100)]
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
        
        public int? billflowid { get; set; }

        [StringLength(500)]
        public string backbilltime { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [Column(TypeName = "ntext")]
        public string paycomment { get; set; }

        [StringLength(500)]
        public string chknote { get; set; }

        [StringLength(20)]
        public string delvcno { get; set; }

        public DateTime? exppaydate { get; set; }

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
