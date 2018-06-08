namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("resthourlog")]
    public partial class resthourlog
    {
        [Key]
        public int rsid { get; set; }

        public int? otlogid { get; set; }

        [StringLength(20)]
        public string osno { get; set; }

        [StringLength(2)]
        public string rstype { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        public float? resthour { get; set; }

        public float? usehour { get; set; }

        public float? moneyhour { get; set; }

        public float? moneyh1 { get; set; }

        public float? moneyh2 { get; set; }

        public float? moneyh3 { get; set; }

        [StringLength(2)]
        public string ifdinner { get; set; }

        public DateTime? adddate { get; set; }

        public DateTime? mydeaddate { get; set; }

        public DateTime? rsdeaddate { get; set; }

        [StringLength(2)]
        public string usetype { get; set; }

        public int? resmoney { get; set; }

        [StringLength(2)]
        public string ottype { get; set; }

        [StringLength(200)]
        public string restcomment { get; set; }

        [StringLength(2)]
        public string ifactive { get; set; }

        [StringLength(2)]
        public string inout { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
