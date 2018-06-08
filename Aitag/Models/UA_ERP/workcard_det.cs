namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class workcard_det
    {
        [Key]
        public int wkdid { get; set; }

        public DateTime? wkcdate { get; set; }

        [StringLength(2)]
        public string wkstep { get; set; }

        [StringLength(20)]
        public string getman { get; set; }

        public DateTime? prdate { get; set; }

        public DateTime? realdate { get; set; }

        [StringLength(10)]
        public string worktype { get; set; }

        [StringLength(10)]
        public string makeitem { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(20)]
        public string wno { get; set; }
    }
}
