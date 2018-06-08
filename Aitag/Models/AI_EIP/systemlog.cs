namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("systemlog")]
    public partial class systemlog
    {
        [Key]
        public int slid { get; set; }

        [StringLength(20)]
        public string slaccount { get; set; }

        [StringLength(50)]
        public string sname { get; set; }

        [Column(TypeName = "ntext")]
        public string slevent { get; set; }

        public DateTime? sldate { get; set; }

        public DateTime? sodate { get; set; }

        [StringLength(50)]
        public string sfromip { get; set; }

        [StringLength(1)]
        public string sflag { get; set; }

        [StringLength(10)]
        public string comid { get; set; }
    }
}
