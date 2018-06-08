namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pollanser")]
    public partial class pollanser
    {
        [Key]
        public int aid { get; set; }

        [StringLength(20)]
        public string sid { get; set; }

        public int? paid { get; set; }

        public int? pid { get; set; }

        [StringLength(50)]
        public string panswer { get; set; }

        public DateTime? pdate { get; set; }

        [Column(TypeName = "ntext")]
        public string pcomment { get; set; }
    }
}
