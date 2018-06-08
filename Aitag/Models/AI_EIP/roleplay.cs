namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("roleplay")]
    public partial class roleplay
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string rid { get; set; }

        [StringLength(50)]
        public string roletitle { get; set; }

        [StringLength(2)]
        public string ifdispatch { get; set; }

        [StringLength(10)]
        public string bossrid { get; set; }

        [StringLength(2)]
        public string ifdptboss { get; set; }

        [StringLength(1000)]
        public string allrid { get; set; }

        [Column(Order = 1)]
        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(1)]
        public string ifrtype { get; set; }

        [StringLength(2)]
        public string hourgroup { get; set; }
    }
}
