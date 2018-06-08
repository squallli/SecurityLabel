namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bag")]
    public partial class bag
    {
        [Key]
        public int bgid { get; set; }

        [StringLength(30)]
        public string bgtitle { get; set; }

        [StringLength(10)]
        public string bgcode { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? bgkg { get; set; }

        [StringLength(20)]
        public string citemid { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
