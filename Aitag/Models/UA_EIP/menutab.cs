namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("menutab")]
    public partial class menutab
    {
        [Key]
        [StringLength(10)]
        public string mtid { get; set; }

        [StringLength(100)]
        public string mttitle { get; set; }

        [StringLength(1)]
        public string mopen { get; set; }

        [StringLength(200)]
        public string location { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(200)]
        public string location1 { get; set; }
    }
}
