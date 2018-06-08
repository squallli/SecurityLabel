namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("carea")]
    public partial class carea
    {
        [Key]
        [StringLength(3)]
        public string cid { get; set; }

        [StringLength(50)]
        public string cname { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(20)]
        public string comid { get; set; }
    }
}
