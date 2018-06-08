namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("empfunction")]
    public partial class empfunction
    {
        [Key]
        public int empdid { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(2)]
        public string funid { get; set; }

        [StringLength(2)]
        public string funposition { get; set; }

        public int? funorder { get; set; }

        [StringLength(2)]
        public string ifshowalert { get; set; }

        public int? funrowcount { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
