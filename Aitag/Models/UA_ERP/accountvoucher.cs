namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("accountvoucher")]
    public partial class accountvoucher
    {
        [Key]
        public int voudid { get; set; }

        public DateTime? voudate { get; set; }

        [StringLength(2)]
        public string voustatus { get; set; }

        [StringLength(20)]
        public string projno { get; set; }

        [StringLength(20)]
        public string vcsno { get; set; }

        public int? deliveryno { get; set; }

        public DateTime? pinvdate { get; set; }

        public int? itemno { get; set; }

        [StringLength(20)]
        public string vouno { get; set; }

        [StringLength(20)]
        public string subjectcode { get; set; }

        [StringLength(100)]
        public string subjectname { get; set; }

        [StringLength(1000)]
        public string vouabstract { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? debitsum { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? creditsum { get; set; }

        [StringLength(10)]
        public string cedeptcode { get; set; }

        [StringLength(10)]
        public string obtno { get; set; }

        [StringLength(20)]
        public string objno { get; set; }

        [StringLength(20)]
        public string cebankaccounts { get; set; }

        [StringLength(20)]
        public string cebillcode { get; set; }

        public DateTime? ceenddate { get; set; }

        [StringLength(20)]
        public string projectno { get; set; }

        [StringLength(20)]
        public string currencytype { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? oricursum { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? exchangerate { get; set; }

        [StringLength(20)]
        public string matnum { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? matamn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? matprice { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? cmatprice { get; set; }

        [StringLength(1)]
        public string specialflag { get; set; }

        [StringLength(1)]
        public string rzflag { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(1)]
        public string ifdel { get; set; }

        [StringLength(20)]
        public string slno { get; set; }

        public int? slyear { get; set; }

        public int? slmonth { get; set; }

        public DateTime? sldate { get; set; }

        [StringLength(2)]
        public string billtype { get; set; }

        [StringLength(2)]
        public string vcsubtype { get; set; }
        [StringLength(2)]
        public string costtype { get; set; }
        [StringLength(20)]
        public string ownman { get; set; }
    }
}
