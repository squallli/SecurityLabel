namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("schedule")]
    public partial class schedule
    {
        [Key]
        public int schid { get; set; }

        public int? schcparentid { get; set; }

        [StringLength(2)]
        public string schworkstatus { get; set; }

        [StringLength(1)]
        public string schmeetstatus { get; set; }

        [StringLength(2)]
        public string schtype { get; set; }

        [StringLength(200)]
        public string pretitle { get; set; }

        [StringLength(2000)]
        public string precontent { get; set; }

        [StringLength(200)]
        public string schtitle { get; set; }

        [StringLength(4000)]
        public string schcontent { get; set; }

        [StringLength(100)]
        public string schplace { get; set; }

        public DateTime? schdate { get; set; }

        [StringLength(2)]
        public string schhour { get; set; }

        [StringLength(2)]
        public string schmin { get; set; }

        public DateTime? schedate { get; set; }

        [StringLength(2)]
        public string schehour { get; set; }

        [StringLength(2)]
        public string schemin { get; set; }

        public DateTime? schdonedate { get; set; }

        [StringLength(2)]
        public string schdonehour { get; set; }

        [StringLength(2)]
        public string schdonemin { get; set; }

        [StringLength(20)]
        public string schloginer { get; set; }

        [StringLength(20)]
        public string schchief { get; set; }

        [StringLength(20)]
        public string schowner { get; set; }

        [StringLength(20)]
        public string schexecer { get; set; }

        [StringLength(200)]
        public string schsub { get; set; }

        [StringLength(200)]
        public string masterid { get; set; }

        [StringLength(200)]
        public string subid { get; set; }

        [StringLength(200)]
        public string otherid { get; set; }

        [StringLength(100)]
        public string mrid { get; set; }

        [StringLength(100)]
        public string resid { get; set; }

        [StringLength(50)]
        public string meetfile { get; set; }

        [StringLength(500)]
        public string meetman { get; set; }

        [StringLength(500)]
        public string schworkcontent { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? workhour { get; set; }

        [StringLength(10)]
        public string workclass { get; set; }

        [StringLength(30)]
        public string projid { get; set; }

        [StringLength(10)]
        public string stepid { get; set; }

        [Column(TypeName = "ntext")]
        public string readallman { get; set; }

        public int? sid { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        public DateTime? presdate { get; set; }

        public DateTime? preedate { get; set; }

        [StringLength(1)]
        public string stype { get; set; }

        [StringLength(50)]
        public string sclass { get; set; }

        [StringLength(100)]
        public string slink { get; set; }

        [StringLength(50)]
        public string sfile { get; set; }
        
    }
}
