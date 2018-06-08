namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mediaclass")]
    public partial class mediaclass
    {
        [Key]
        [StringLength(20)]
        public string mcno { get; set; }

        [StringLength(50)]
        public string mctitle { get; set; }

        [StringLength(50)]
        public string msctitle { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? mdrate { get; set; }
    }
}
