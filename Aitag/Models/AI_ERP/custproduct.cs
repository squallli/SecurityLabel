namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("custproduct")]
    public partial class custproduct
    {
        [Key]
        [StringLength(20)]
        public string prodid { get; set; }

        [StringLength(50)]
        public string prodtitle { get; set; }

        [StringLength(20)]
        public string allcomid { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
        [StringLength(20)]
        public string fprodid { get; set; }
    }
}
