namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Checkcode")]
    public partial class Checkcode
    {
        [Key]
        public int cid { get; set; }

        [StringLength(2)]
        public string chkclass { get; set; }

        [StringLength(50)]
        public string chkclasstitle { get; set; }

        [StringLength(2)]
        public string chkcode { get; set; }

        [StringLength(100)]
        public string chkitem { get; set; }

        public int? corder { get; set; }

        [StringLength(2)]
        public string ifspecial { get; set; }

        [StringLength(50)]
        public string specialflag { get; set; }

        [StringLength(100)]
        public string spcomment { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
