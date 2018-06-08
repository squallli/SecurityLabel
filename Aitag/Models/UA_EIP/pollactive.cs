namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pollactive")]
    public partial class pollactive
    {
        [Key]
        public int paid { get; set; }

        [StringLength(100)]
        public string paname { get; set; }

        [StringLength(500)]
        public string pamemo { get; set; }

        public DateTime? pasdate { get; set; }

        public DateTime? paedate { get; set; }

        [StringLength(1)]
        public string ifname { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [Column(TypeName = "ntext")]
        public string readallman { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
