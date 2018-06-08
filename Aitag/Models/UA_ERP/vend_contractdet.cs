namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vend_contractdet
    {
        [Key]
        public int vcdid { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? vcid { get; set; }

        [StringLength(20)]
        public string bdprodno { get; set; }

        [StringLength(100)]
        public string bdprodtitle { get; set; }

        [StringLength(20)]
        public string mdno { get; set; }

        [StringLength(50)]
        public string mdcomment { get; set; }

        [StringLength(2)]
        public string vctype { get; set; }

        [StringLength(20)]
        public string vcno { get; set; }

        public int? vitemno { get; set; }

        [StringLength(20)]
        public string pdunit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vccount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vcmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vcallmoney { get; set; }

        [StringLength(200)]
        public string vcdcomment { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(20)]
        public string ctno { get; set; }

        [StringLength(20)]
        public string pdno { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
