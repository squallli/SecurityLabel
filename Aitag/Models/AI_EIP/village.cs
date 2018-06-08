namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("village")]
    public partial class village
    {
        [Key]
        [StringLength(10)]
        public string vcode { get; set; }

        [StringLength(50)]
        public string vtitle { get; set; }

        [StringLength(3)]
        public string cid { get; set; }

        [StringLength(5)]
        public string postcode { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
