namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("emphdlog")]
    public partial class emphdlog
    {
        [Key]
        public int hid { get; set; }

        public int slyear { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(50)]
        public string empname { get; set; }

        public DateTime? factsday { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        [StringLength(10)]
        public string hdayid { get; set; }

        public DateTime? effectiveday { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? allhour { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? usehour { get; set; }

        [StringLength(1)]
        public string hchange { get; set; }

        [Required]
        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? careeryear { get; set; }
    }
}
