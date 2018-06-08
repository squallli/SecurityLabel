namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("progparam")]
    public partial class progparam
    {
        [Key]
        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(2)]
        public string ifshowcount { get; set; }

        [StringLength(10)]
        public string systime { get; set; }

        [StringLength(10)]
        public string mailtime { get; set; }

        [StringLength(1)]
        public string ifkickuser { get; set; }

        public int? pollopcount { get; set; }

        public int? photofile { get; set; }

        public int? emailquota { get; set; }

        [StringLength(50)]
        public string pop3server { get; set; }

        [StringLength(50)]
        public string smtpserver { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        public int? stopholidayyear { get; set; }

        public int? accday1 { get; set; }

        public int? accday2 { get; set; }

        public int? accday3 { get; set; }

        public int? accday4 { get; set; }

        public int? accday5 { get; set; }

        [StringLength(100)]
        public string pgmail1 { get; set; }

        [StringLength(100)]
        public string pgmail2 { get; set; }

        [StringLength(100)]
        public string pgmail3 { get; set; }

        [StringLength(2)]
        public string ifclose { get; set; }
    }
}
