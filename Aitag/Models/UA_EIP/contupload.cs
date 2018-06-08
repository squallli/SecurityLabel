namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("contupload")]
    public partial class contupload
    {
        [Key]
        public int cupid { get; set; }

        public int? cfilesize { get; set; }

        [StringLength(500)]
        public string cfilename { get; set; }

        [StringLength(100)]
        public string cupfile { get; set; }

        [StringLength(500)]
        public string cfiletitle { get; set; }

        public int? mcid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
