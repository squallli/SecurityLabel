namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("farmer")]
    public partial class farmer
    {
        [Key]
        [StringLength(20)]
        public string farmerno { get; set; }

        [StringLength(50)]
        public string farmername { get; set; }

        [StringLength(10)]
        public string vcode { get; set; }

        [StringLength(50)]
        public string ftel { get; set; }

        [StringLength(50)]
        public string fmob { get; set; }

        [StringLength(200)]
        public string faddr { get; set; }

        [StringLength(500)]
        public string fcomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
