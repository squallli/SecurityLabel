namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subreadwrite")]
    public partial class subreadwrite
    {
        [Key]
        public int srwid { get; set; }
        public int? sid { get; set; }

        [Column(TypeName = "ntext")]
        public string dptgroup { get; set; }

        [Column(TypeName = "ntext")]
        public string empgroup { get; set; }

        [StringLength(1)]
        public string subread { get; set; }

        [StringLength(1)]
        public string subadd { get; set; }

        [StringLength(1)]
        public string submod { get; set; }

        [StringLength(1)]
        public string subdel { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string BMODID { get; set; }

        public DateTime? BMODDATE { get; set; }
    }
}
