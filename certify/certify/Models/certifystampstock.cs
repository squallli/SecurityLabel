namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("certifystampstock")]
    public partial class certifystampstock
    {
        [Key]
        public int csdid { get; set; }

        [StringLength(2)]
        public string csstatus { get; set; }

        public int? csid { get; set; }

        [StringLength(30)]
        public string cerno { get; set; }

        [StringLength(30)]
        public string barcode { get; set; }

        public int? cstime { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(30)]
        public string codno
        {
            get;
            set;
        }

        public int? codid
        {
            get;
            set;
        }

        public string ifrand { get; set; }
    }
}
