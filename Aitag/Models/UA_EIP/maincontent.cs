namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("maincontent")]
    public partial class maincontent
    {
        [Key]
        public int mcid { get; set; }

        [StringLength(2)]
        public string mcfiletype { get; set; }

        [StringLength(2)]
        public string mctype { get; set; }

        [StringLength(300)]
        public string mctitle { get; set; }

        public int? mcparentid { get; set; }

        [StringLength(200)]
        public string mchttp { get; set; }

        [Column(TypeName = "ntext")]
        public string mccontent { get; set; }

        [Column(TypeName = "ntext")]
        public string othercontent { get; set; }

        public int? mclassid { get; set; }

        public int? sid { get; set; }

        [StringLength(20)]
        public string ownman { get; set; }

        public DateTime? mdate { get; set; }

        public DateTime? mreplydate { get; set; }

        public int? mclick { get; set; }

        [Column(TypeName = "ntext")]
        public string readallman { get; set; }

        [StringLength(100)]
        public string mcplace { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        public int? pmcid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(100)]
        public string pname { get; set; }

        [StringLength(30)]
        public string projid { get; set; }

        [StringLength(10)]
        public string stepid { get; set; }

        public DateTime? msdate { get; set; }

        public DateTime? medate { get; set; }
    }
}
