namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("billflow")]
    public partial class billflow
    {
        [Key]
        public int bid { get; set; }

        public int? flowscount { get; set; }

        public int? flowecount { get; set; }

        [StringLength(20)]
        public string addr { get; set; }

        [StringLength(20)]
        public string billid { get; set; }

        [StringLength(100)]
        public string flowcheck { get; set; }

        public int? flowlevel { get; set; }

        public int? jumpday { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        public int? btop { get; set; }

        [StringLength(10)]
        public string comclass { get; set; }

       [StringLength(1)]
        public string ifuse { get; set; }

        [StringLength(500)]
        public string billtype { get; set; }
        
    }
}
