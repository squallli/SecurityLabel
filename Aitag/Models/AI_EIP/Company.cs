namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Company")]
    public partial class Company
    {
        [Key]
        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(1)]
        public string cifwebmail { get; set; }

        [StringLength(1)]
        public string cflag { get; set; }

        public int? cbtype { get; set; }

        public int? cstype { get; set; }

        [StringLength(100)]
        public string comtitle { get; set; }

        [StringLength(50)]
        public string comsttitle { get; set; }

        [StringLength(100)]
        public string comhttp { get; set; }

        [StringLength(20)]
        public string comtel { get; set; }

        [StringLength(20)]
        public string comfax { get; set; }

        [StringLength(100)]
        public string comadd { get; set; }

        public DateTime? cedate { get; set; }

        public int? caddcount { get; set; }

        [Column(TypeName = "ntext")]
        public string cnote { get; set; }

        [Column(TypeName = "ntext")]
        public string cprofile { get; set; }

        [Column(TypeName = "ntext")]
        public string cservice { get; set; }

        [Column(TypeName = "ntext")]
        public string chistory { get; set; }

        [Column(TypeName = "ntext")]
        public string cprod { get; set; }

        [StringLength(200)]
        public string ckeyword { get; set; }

        public DateTime? cadate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? comclick { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(10)]
        public string csno { get; set; }

        [StringLength(10)]
        public string ctype { get; set; }

        [StringLength(100)]
        public string cplink { get; set; }

        [StringLength(10)]
        public string pcomid { get; set; }

        [StringLength(1)]
        public string cmcode { get; set; }

        [StringLength(50)]
        public string logopic { get; set; }
    }
}
