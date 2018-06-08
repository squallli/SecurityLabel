namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cardreallog")]
    public partial class cardreallog
    {
        [Key]
        public int crid { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(20)]
        public string empname { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        public DateTime? clogdate { get; set; }

        [StringLength(20)]
        public string clogtime2 { get; set; }

        [StringLength(20)]
        public string clogtime { get; set; }

        [StringLength(500)]
        public string clogcomment { get; set; }

        [StringLength(2)]
        public string indbstatus { get; set; }

        [StringLength(20)]
        public string filename { get; set; }

        [StringLength(2)]
        public string functype { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(30)]
        public string tmpcardno { get; set; }

        [StringLength(50)]
        public string tmpdepid { get; set; }
    }
}
