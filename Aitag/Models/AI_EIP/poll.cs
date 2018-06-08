namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("poll")]
    public partial class poll
    {
        [Key]
        public int pid { get; set; }

        public int? paid { get; set; }

        [StringLength(2)]
        public string pno { get; set; }

        [StringLength(200)]
        public string pname { get; set; }

        [StringLength(1)]
        public string ptype { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
