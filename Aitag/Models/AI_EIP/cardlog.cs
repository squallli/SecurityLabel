namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cardlog")]
    public partial class cardlog
    {
        [Key]
        public int clogid { get; set; }

        [StringLength(20)]
        public string csno { get; set; }

        [StringLength(2)]
        public string clogstatus { get; set; }

        [StringLength(2)]
        public string clogreason { get; set; }

        [StringLength(2)]
        public string clogtype { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(20)]
        public string empname { get; set; }

        [StringLength(10)]
        public string dptid { get; set; }

        [StringLength(200)]
        public string roledptid { get; set; }

        [StringLength(20)]
        public string arolestampid { get; set; }

        [StringLength(20)]
        public string rolestampid { get; set; }

        [StringLength(200)]
        public string rolestampidall { get; set; }

        [StringLength(200)]
        public string empstampidall { get; set; }

        public DateTime? clogdate { get; set; }

        [StringLength(4)]
        public string clogtime { get; set; }

        [StringLength(4)]
        public string clogtime1 { get; set; }

        [StringLength(500)]
        public string clogcomment { get; set; }

        public DateTime? cardbsdate { get; set; }

        public DateTime? cardbedate { get; set; }

        [StringLength(20)]
        public string tmpcardno { get; set; }

        [StringLength(200)]
        public string tmpcardcomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        public DateTime? cdate { get; set; }

        [StringLength(200)]
        public string billtime { get; set; }

        [StringLength(500)]
        public string cback { get; set; }

        public int? billflowid { get; set; }

        [StringLength(200)]
        public string empmeetsign { get; set; }
    }
}
