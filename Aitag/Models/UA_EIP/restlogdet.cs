namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("restlogdet")]
    public partial class restlogdet
    {
        [Key]
        public int hsid { get; set; }

        [StringLength(20)]
        public string hsno { get; set; }

        public int? rsid { get; set; }

        public float? usehour { get; set; }

        [StringLength(20)]
        public string empid { get; set; }
    }
}
