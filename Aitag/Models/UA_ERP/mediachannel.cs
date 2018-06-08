namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mediachannel")]
    public partial class mediachannel
    {
        [Key]
        [StringLength(20)]
        public string mdno { get; set; }

        [StringLength(100)]
        public string mdtitle { get; set; }

        [StringLength(20)]
        public string pmdno { get; set; }
 
     
        [StringLength(20)]
        public string mcno { get; set; }

        [StringLength(20)]
        public string allcomid { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
