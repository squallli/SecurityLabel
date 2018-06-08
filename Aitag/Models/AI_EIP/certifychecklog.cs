namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("certifychecklog")]
    public partial class certifychecklog
    {
        [Key]
        public int clid { get; set; }

        [Required]
        [StringLength(30)]
        public string cerno { get; set; }

        [StringLength(2)]
        public string clstatus { get; set; }

        public DateTime? cldate { get; set; }

        [StringLength(500)]
        public string clcomment { get; set; }

        [StringLength(20)]
        public string ownman { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
