namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mainclass")]
    public partial class mainclass
    {
        [Key]
        public int mclassid { get; set; }

        [StringLength(100)]
        public string mclasstitle { get; set; }

        public int? sid { get; set; }

        public int? mcorder { get; set; }

        [StringLength(20)]
        public string owner { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
