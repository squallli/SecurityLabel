namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("polldetail")]
    public partial class polldetail
    {
        [Key]
        public int pdid { get; set; }

        public int? pid { get; set; }

        [StringLength(2)]
        public string pno { get; set; }

        [StringLength(100)]
        public string ptitle { get; set; }

        public int? pcount { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
