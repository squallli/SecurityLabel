namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Department")]
    public partial class Department
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string dptid { get; set; }

        [StringLength(50)]
        public string dpttitle { get; set; }

        [StringLength(20)]
        public string accdptid { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
