namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("privrole")]
    public partial class privrole
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string msid { get; set; }

        [StringLength(50)]
        public string mstitle { get; set; }

        [Column(Order = 1)]
        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
