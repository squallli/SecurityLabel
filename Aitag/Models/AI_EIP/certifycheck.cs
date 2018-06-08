namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("certifycheck")]
    public partial class certifycheck
    {
        [Key]
        [StringLength(30)]
        public string cerno { get; set; }

        [StringLength(2)]
        public string cstatus { get; set; }

        [StringLength(20)]
        public string farmerno { get; set; }

        [StringLength(20)]
        public string certime { get; set; }
        
        public DateTime? adddate { get; set; }

        public DateTime? cerdate { get; set; }

        public DateTime? dealdate { get; set; }
        public DateTime? givedate { get; set; }

        [StringLength(500)]
        public string cercomment { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
