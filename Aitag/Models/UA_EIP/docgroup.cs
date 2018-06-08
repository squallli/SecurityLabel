namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("docgroup")]
    public partial class docgroup
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string docid { get; set; }

        [StringLength(50)]
        public string doctitle { get; set; }

        [StringLength(2)]
        public string doctype { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(10)]
        public string docflag { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [Column(TypeName = "ntext")]
        public string doctext { get; set; }
    }
}
