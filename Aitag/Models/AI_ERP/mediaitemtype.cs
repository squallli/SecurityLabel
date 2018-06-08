namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mediaitemtype")]
    public partial class mediaitemtype
    {
        [Key]
        [StringLength(20)]
        public string mino { get; set; }

        [StringLength(20)]
        public string mitype { get; set; }

        [StringLength(200)]
        public string mcno { get; set; }

        [StringLength(100)]
        public string mititle { get; set; }

        [StringLength(50)]
        public string mdtitle { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
