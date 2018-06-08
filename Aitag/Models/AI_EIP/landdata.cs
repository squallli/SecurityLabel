namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("landdata")]
    public partial class landdata
    {
        [Key]
        public int ldno { get; set; }

        [StringLength(5)]
        public string lcode { get; set; }

        [StringLength(4)]
        public string landno1 { get; set; }

        [StringLength(4)]
        public string landno2 { get; set; }

        [StringLength(50)]
        public string ownman { get; set; }

        [StringLength(50)]
        public string rentman { get; set; }

        [StringLength(500)]
        public string lcomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
