namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("battadet")]
    public partial class battadet
    {
        [Key]
        public int bdid { get; set; }

        public int? blogid { get; set; }

        [StringLength(50)]
        public string bdmonth { get; set; }

        [StringLength(50)]
        public string bdday { get; set; }

        public DateTime? bddate { get; set; }

        [StringLength(50)]
        public string bdplace { get; set; }

        [StringLength(50)]
        public string bdwork { get; set; }

        public int? bdplane { get; set; }

        public int? bdcar { get; set; }

        public int? bdtrain { get; set; }

        public int? bdship { get; set; }

        public int? bdliving1 { get; set; }

        public int? bdliving2 { get; set; }

        public int? bdother { get; set; }

        public int? bland { get; set; }

        public int? blive { get; set; }

        public int? bvisa { get; set; }

        public int? binsurance { get; set; }

        public int? badmin { get; set; }

        public int? bgift { get; set; }

        [StringLength(50)]
        public string bdbillno { get; set; }

        [StringLength(200)]
        public string bdcomment { get; set; }
    }
}
