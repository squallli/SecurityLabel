namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class custpurchase_det
    {
        [Key]   
        public int pdid { get; set; }

        [StringLength(20)]
        public string bdprodno { get; set; }

        [StringLength(100)]
        public string bdprodtitle { get; set; }

        [StringLength(20)]
        public string mdno { get; set; }

        [StringLength(50)]
        public string mdcomment { get; set; }
        
        [StringLength(20)]
        public string pid { get; set; }

        [StringLength(20)]
        public string pdno { get; set; }

        public int? pitemno { get; set; }

        [StringLength(20)]
        public string pdunit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pdcount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pdmoney { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? pdallmoney { get; set; }

        [StringLength(200)]
        public string pdcomment { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(20)]
        public string ctno { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(10)]
        public string allcomid { get; set; }

    }
}
