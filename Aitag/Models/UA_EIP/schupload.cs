namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("schupload")]
    public partial class schupload
    {
        [Key]
        public int schupid { get; set; }

        public int? schfilesize { get; set; }

        [StringLength(100)]
        public string schfilename { get; set; }

        [StringLength(50)]
        public string schupfile { get; set; }

        public int? schid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
