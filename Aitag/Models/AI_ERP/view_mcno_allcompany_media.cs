namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class view_mcno_allcompany_media
    {
        [Key]
        public int cid { get; set; }

        [StringLength(20)]
        public string mdno { get; set; }

        [StringLength(50)]
        public string contactman { get; set; }

        [StringLength(50)]
        public string ctel { get; set; }

        [StringLength(50)]
        public string cmob { get; set; }

        [StringLength(50)]
        public string cfax { get; set; }

        [StringLength(10)]
        public string allcomid { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(20)]
        public string mcno { get; set; }

        public DateTime? ctsdate { get; set; }
        public DateTime? ctedate { get; set; }
    }
}
