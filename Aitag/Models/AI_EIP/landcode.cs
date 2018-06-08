namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("landcode")]
    public partial class landcode
    {
        [Key]
        [StringLength(5)]
        public string lcode { get; set; }

        [StringLength(20)]
        public string ltitle { get; set; }

        [StringLength(10)]
        public string vcode { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
