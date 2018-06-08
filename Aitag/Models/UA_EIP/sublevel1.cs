namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sublevel1
    {
        [Key]
        public int sid { get; set; }

        public int? corder { get; set; }

        [StringLength(2)]
        public string stype { get; set; }

        [StringLength(1)]
        public string lid { get; set; }

        [StringLength(50)]
        public string sublevelname { get; set; }

        public int? uplink { get; set; }

        [StringLength(200)]
        public string location { get; set; }

        [StringLength(10)]
        public string psid { get; set; }

        [StringLength(1)]
        public string property1 { get; set; }

        [StringLength(1)]
        public string subread { get; set; }

        [StringLength(1)]
        public string subadd { get; set; }

        [StringLength(1)]
        public string submod { get; set; }

        [StringLength(1)]
        public string subdel { get; set; }

        [StringLength(2)]
        public string counttype { get; set; }

        [StringLength(10)]
        public string docid { get; set; }

        [StringLength(20)]
        public string BMODID { get; set; }

        public DateTime? BMODDATE { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(2)]
        public string functype { get; set; }

        [StringLength(1)]
        public string ifname { get; set; }

        [StringLength(10)]
        public string mtid { get; set; }

        [StringLength(2)]
        public string ifshow { get; set; }

        [StringLength(20)]
        public string empmagid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? filequota { get; set; }

        [StringLength(2)]
        public string filecontrol { get; set; }

        [StringLength(200)]
        public string location1 { get; set; }
    }
}
