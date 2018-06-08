namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class incomeaccounts
    {
        [Key]
        public int icid { get; set; }

        [StringLength(20)]
        public string invno { get; set; }

        [StringLength(2)]
        public string invtype { get; set; }

        public DateTime? invdate { get; set; }

        [StringLength(50)]
        public string cusserno { get; set; }

        [StringLength(2)]
        public string taxtype { get; set; }

        [StringLength(100)]
        public string catcomment { get; set; }

        public int? catcount { get; set; }

        [StringLength(2)]
        public string format { get; set; }

        public int? invmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? invtax { get; set; }

        public int? salemoney { get; set; }

        public int? taxmoney { get; set; }

        [StringLength(10)]
        public string taxzone { get; set; }

        [StringLength(100)]
        public string othercomment { get; set; }

        public int? vcinvid { get; set; }

        [StringLength(20)]
        public string vcno { get; set; }

        public int? vserno { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
