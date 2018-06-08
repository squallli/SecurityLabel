namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("emprole")]
    public partial class emprole
    {
        [Key]
        public int erid { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(10)]
        public string rid { get; set; }

        [StringLength(2)]
        public string ridtype { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(10)]
        public string comid { get; set; }
    }
}
