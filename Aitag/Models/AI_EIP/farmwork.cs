namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("farmwork")]
    public partial class farmwork
    {
        [Key]
        public int fwid { get; set; }

        [StringLength(20)]
        public string farmerno { get; set; }

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
