namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("postcode")]
    public partial class postcode
    {
        [Key]
        [Column("postcode")]
        [StringLength(3)]
        public string postcode1 { get; set; }

        [StringLength(3)]
        public string city { get; set; }

        [StringLength(10)]
        public string zonename { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(10)]
        public string deptid { get; set; }
    }
}
