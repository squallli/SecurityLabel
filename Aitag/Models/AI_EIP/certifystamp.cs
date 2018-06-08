namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("certifystamp")]
    public partial class certifystamp
    {
        [Key]
        public int csid { get; set; }

        [StringLength(20)]
        public string citemid { get; set; }

        [StringLength(2)]
        public string wtrack { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? sno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? eno { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cscount { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
