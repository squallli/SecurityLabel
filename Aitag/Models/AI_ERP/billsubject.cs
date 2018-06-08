namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("billsubject")]
    public partial class billsubject
    {
        [Key]
        public int accid { get; set; }

        [Required]

       // [StringLength(10)]
       // public string comtype { get; set; }

        [StringLength(10)]
        public string comclass { get; set; }

        [StringLength(10)]
        public string billtype { get; set; }

        [StringLength(20)]
        public string itemcode { get; set; }

        [StringLength(20)]
        public string oldsubjectcode { get; set; }

        [StringLength(20)]
        public string subjectcode { get; set; }

        [StringLength(200)]
        public string subjecttitle { get; set; }

        [StringLength(1)]
        public string acget { get; set; }

        public int? hourcost { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
