namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("yearhddet")]
    public partial class yearhddet
    {
        [Key]
        public int hid { get; set; }

        [Required]
        [StringLength(10)]
        public string yhid { get; set; }

        [StringLength(10)]
        public string hdayid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? allhour { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
