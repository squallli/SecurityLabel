namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("certifycheckdet")]
    public partial class certifycheckdet
    {
        [Key]
        public int cerdid { get; set; }

        [StringLength(30)]
        public string cerno { get; set; }

        public int? ldno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fwarea { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
